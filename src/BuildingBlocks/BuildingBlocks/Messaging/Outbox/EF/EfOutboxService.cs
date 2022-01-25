using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.EFCore;
using Humanizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private readonly IOutboxDataContext _outboxDataContext;

    public EfOutboxService(
        IOptions<OutboxOptions> options,
        ILogger<EfOutboxService<TContext>> logger,
        IMessageSerializer messageSerializer,
        IMediator mediator,
        IBusPublisher busPublisher,
        IOutboxDataContext outboxDataContext)
    {
        _options = options.Value;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _mediator = mediator;
        _busPublisher = busPublisher;
        _outboxDataContext = outboxDataContext;
    }

    public async Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        var messages = await _outboxDataContext.OutboxMessages
            .Where(x => x.EventType == eventType && x.ProcessedOn == null)
            .ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        var messages = await _outboxDataContext.OutboxMessages
            .Where(x => x.EventType == eventType).ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _outboxDataContext.OutboxMessages
            .Where(x => x.ProcessedOn != null).ToListAsync(cancellationToken: cancellationToken);

        _outboxDataContext.OutboxMessages.RemoveRange(messages);
        await _outboxDataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(
        CancellationToken cancellationToken = default,
        params IIntegrationEvent[] integrationEvents)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
        }

        foreach (var integrationEvent in integrationEvents)
        {
            string name = integrationEvent.GetType().Name;

            var outboxMessages = new OutboxMessage(
                integrationEvent.EventId,
                integrationEvent.OccurredOn,
                integrationEvent.GetType().AssemblyQualifiedName,
                name.Underscore(),
                _messageSerializer.Serialize(integrationEvent),
                EventType.IntegrationEvent,
                correlationId: null);

            await _outboxDataContext.OutboxMessages.AddAsync(outboxMessages, cancellationToken);
        }

        await _outboxDataContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved messages to the outbox");
    }

    public async Task SaveAsync(
        CancellationToken cancellationToken = default,
        params IDomainNotificationEvent[] domainNotificationEvents)
    {
        Guard.Against.Null(domainNotificationEvents, nameof(domainNotificationEvents));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
        }

        foreach (var domainNotificationEvent in domainNotificationEvents)
        {
            string name = domainNotificationEvent.GetType().Name;

            var outboxMessages = new OutboxMessage(
                domainNotificationEvent.EventId,
                domainNotificationEvent.OccurredOn,
                domainNotificationEvent.GetType().AssemblyQualifiedName,
                name.Underscore(),
                _messageSerializer.Serialize(domainNotificationEvent),
                EventType.DomainNotificationEvent,
                correlationId: null);

            await _outboxDataContext.OutboxMessages.AddAsync(outboxMessages);
            await _outboxDataContext.SaveChangesAsync();
        }

        _logger.LogInformation("Saved messages to the outbox");
    }

    public async Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = await _outboxDataContext.OutboxMessages
            .Where(x => x.ProcessedOn == null).ToListAsync(cancellationToken: cancellationToken);

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

            dynamic data = _messageSerializer.Deserialize(outboxMessage.Data, type);
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

        await _outboxDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
