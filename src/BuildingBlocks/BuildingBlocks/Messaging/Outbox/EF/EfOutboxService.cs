using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.EFCore;
using BuildingBlocks.Messaging.Serialization;
using Humanizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.Outbox.EF;

public class EfOutboxService<TContext> : IOutboxService
    where TContext : AppDbContextBase
{
    private readonly OutboxOptions _options;
    private readonly ILogger<EfOutboxService<TContext>> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IBusPublisher _busPublisher;
    private readonly IMediator _mediator;
    private readonly OutboxDataContext _outboxDataContext;

    public EfOutboxService(
        IOptions<OutboxOptions> options,
        ILogger<EfOutboxService<TContext>> logger,
        IMessageSerializer messageSerializer,
        IBusPublisher busPublisher,
        IMediator mediator,
        OutboxDataContext outboxDataContext)
    {
        _options = options.Value;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _busPublisher = busPublisher;
        _mediator = mediator;
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

    public async Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        _ = _outboxDataContext.Database.GetConnectionString();

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
        }

        string name = integrationEvent.GetType().Name;

        var outboxMessages = new OutboxMessage(
            integrationEvent.EventId,
            integrationEvent.OccurredOn,
            integrationEvent.EventType,
            name.Underscore(),
            _messageSerializer.Serialize(integrationEvent),
            EventType.IntegrationEvent,
            correlationId: null);

        await _outboxDataContext.OutboxMessages.AddAsync(outboxMessages, cancellationToken);
        await _outboxDataContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved message to the outbox");
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
            domainNotificationEvent.EventType,
            name.Underscore(),
            _messageSerializer.Serialize(domainNotificationEvent),
            EventType.DomainNotificationEvent,
            correlationId: null);

        await _outboxDataContext.OutboxMessages.AddAsync(outboxMessages, cancellationToken);
        await _outboxDataContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved message to the outbox");
    }

    public async Task SaveAsync(IInternalCommand internalCommand, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(internalCommand, nameof(internalCommand));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
        }

        string name = internalCommand.GetType().Name;

        var outboxMessages = new OutboxMessage(
            internalCommand.Id,
            internalCommand.OccurredOn,
            internalCommand.CommandType,
            name.Underscore(),
            _messageSerializer.Serialize(internalCommand),
            EventType.InternalCommand,
            correlationId: null);

        await _outboxDataContext.OutboxMessages.AddAsync(outboxMessages, cancellationToken);
        await _outboxDataContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved message to the outbox");
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

            // if (type is null)
            // {
            //     _outboxDataContext.OutboxMessages.Remove(outboxMessage);
            //     await _outboxDataContext.SaveChangesAsync(cancellationToken);
            //     continue;
            // }

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
                    "Dispatched a notification: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    domainNotificationEvent?.EventId);
            }

            if (outboxMessage.EventType == EventType.IntegrationEvent)
            {
                var integrationEvent = data as IIntegrationEvent;

                // integration event
                await _busPublisher.PublishAsync(integrationEvent, cancellationToken);

                _logger.LogInformation(
                    "Publish a message: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    integrationEvent?.EventId);
            }


            if (outboxMessage.EventType == EventType.InternalCommand)
            {
                var internalCommand = data as IInternalCommand;

                await _mediator.Send(internalCommand, cancellationToken);

                _logger.LogInformation(
                    "Sent a internal command: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    internalCommand.Id);
            }

            outboxMessage.MarkAsProcessed();
        }

        await _outboxDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
