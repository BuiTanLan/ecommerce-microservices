using Ardalis.GuardClauses;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.EFCore;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.Outbox;

public class EfOutboxService<TContext> : IOutboxService
    where TContext : AppDbContextBase
{
    private readonly OutboxOptions _options;
    private readonly ILogger<EfOutboxService<TContext>> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMediator _mediator;
    private readonly IBusPublisher _busPublisher;
    private readonly IUnitOfWork<TContext> _unitOfWork;

    public EfOutboxService(
        IOptions<OutboxOptions> options,
        ILogger<EfOutboxService<TContext>> logger,
        IMessageSerializer messageSerializer,
        IMediator mediator,
        IBusPublisher busPublisher,
        IUnitOfWork<TContext> unitOfWork)
    {
        _options = options.Value;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _mediator = mediator;
        _busPublisher = busPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.GetRepository<OutboxMessage>()
            .FindAsync(x => x.EventType == eventType && x.ProcessedOn == null, cancellationToken);

        return messages;
    }

    public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.GetRepository<OutboxMessage>()
            .FindAsync(x => x.EventType == eventType, cancellationToken);

        return messages;
    }

    public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.GetRepository<OutboxMessage>()
            .FindAsync(x => x.ProcessedOn != null, cancellationToken);

        _unitOfWork.GetRepository<OutboxMessage>().DeleteRange(messages);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
        }

        string name = integrationEvent.GetType().Name;

        var outboxMessages = new OutboxMessage(
            integrationEvent.EventId,
            integrationEvent.OccurredOn,
            integrationEvent.GetType().AssemblyQualifiedName,
            name.Underscore(),
            _messageSerializer.Serialize(integrationEvent),
            EventType.IntegrationEvent,
            correlationId: null);

        await _unitOfWork.GetRepository<OutboxMessage>().AddAsync(outboxMessages, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved messages to the outbox");
    }

    public async Task SaveAsync(
        IDomainNotificationEvent domainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(domainNotificationEvent, nameof(domainNotificationEvent));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
        }

        string name = domainNotificationEvent.GetType().Name;

        var outboxMessages = new OutboxMessage(
            domainNotificationEvent.EventId,
            domainNotificationEvent.OccurredOn,
            domainNotificationEvent.GetType().AssemblyQualifiedName,
            name.Underscore(),
            _messageSerializer.Serialize(domainNotificationEvent),
            EventType.DomainNotificationEvent,
            correlationId: null);

        await _unitOfWork.GetRepository<OutboxMessage>().AddAsync(outboxMessages, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved messages to the outbox");
    }

    public async Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = await _unitOfWork.GetRepository<OutboxMessage>()
            .FindAsync(x => x.ProcessedOn == null, cancellationToken);

        if (!unsentMessages.Any())
        {
            _logger.LogTrace("No unsent messages found in outbox");
            return;
        }

        _logger.LogInformation(
            "Found {Count} unsent messages in outbox, sending...",
            unsentMessages.Count);

        foreach (var outboxMessage in unsentMessages)
        {
            var type = Type.GetType(outboxMessage.Type);

            var data = _messageSerializer.Deserialize(outboxMessage.Data, type);
            if (data is null)
            {
                _logger.LogError("Invalid message type: {Name}", type?.Name);
                continue;
            }

            if (outboxMessage.EventType == EventType.DomainNotificationEvent)
            {
                var domainNotificationEvent = data as IDomainNotificationEvent;

                // domain event notification
                await _mediator.Publish(domainNotificationEvent, cancellationToken);

                _logger.LogInformation(
                    "Published a notification: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    domainNotificationEvent?.EventId);
            }

            if (outboxMessage.EventType == EventType.IntegrationEvent)
            {
                var integrationEvent = data as IIntegrationEvent;

                // integration event
                await _busPublisher.PublishAsync(integrationEvent, cancellationToken);

                _logger.LogInformation(
                    "Published a message: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    integrationEvent?.EventId);
            }

            outboxMessage.MarkAsProcessed();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
