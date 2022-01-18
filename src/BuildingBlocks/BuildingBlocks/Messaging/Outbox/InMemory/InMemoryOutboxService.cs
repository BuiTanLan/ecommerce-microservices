using Ardalis.GuardClauses;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.Outbox.InMemory;

public class InMemoryOutboxService : IOutboxService
{
    private readonly OutboxOptions _options;
    private readonly ILogger<InMemoryOutboxService> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMediator _mediator;
    private readonly IBusPublisher _busPublisher;
    private readonly IInMemoryOutboxStore _inMemoryOutboxStore;

    public InMemoryOutboxService(
        IOptions<OutboxOptions> options,
        ILogger<InMemoryOutboxService> logger,
        IMessageSerializer messageSerializer,
        IMediator mediator,
        IBusPublisher busPublisher,
        IInMemoryOutboxStore inMemoryOutboxStore)
    {
        _options = options.Value;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _mediator = mediator;
        _busPublisher = busPublisher;
        _inMemoryOutboxStore = inMemoryOutboxStore;
    }

    public Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        var messages = _inMemoryOutboxStore.Events
            .Where(x => x.EventType == eventType && x.ProcessedOn == null);

        return Task.FromResult(messages);
    }

    public Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent | EventType.DomainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        var messages = _inMemoryOutboxStore.Events.Where(x => x.EventType == eventType);

        return Task.FromResult(messages);
    }

    public Task CleanProcessedAsync(CancellationToken cancellationToken = default)
    {
        _inMemoryOutboxStore.Events.ToList().RemoveAll(x => x.ProcessedOn != null);

        return Task.CompletedTask;
    }

    public Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return Task.CompletedTask;
        }

        string name = integrationEvent.GetType().Name;

        var outboxMessages = new OutboxMessage(
            integrationEvent.Id,
            integrationEvent.OccurredOn,
            integrationEvent.GetType().AssemblyQualifiedName,
            name.Underscore(),
            _messageSerializer.Serialize(integrationEvent),
            EventType.IntegrationEvent,
            correlationId: null);

        _inMemoryOutboxStore.Events.Add(outboxMessages);

        _logger.LogInformation("Saved messages to the outbox");

        return Task.CompletedTask;
    }

    public Task SaveAsync(
        IDomainNotificationEvent domainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(domainNotificationEvent, nameof(domainNotificationEvent));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return Task.CompletedTask;
        }

        string name = domainNotificationEvent.GetType().Name;

        var outboxMessages = new OutboxMessage(
            domainNotificationEvent.Id,
            domainNotificationEvent.OccurredOn,
            domainNotificationEvent.GetType().AssemblyQualifiedName,
            name.Underscore(),
            _messageSerializer.Serialize(domainNotificationEvent),
            EventType.DomainNotificationEvent,
            correlationId: null);

        _inMemoryOutboxStore.Events.Add(outboxMessages);

        _logger.LogInformation("Saved messages to the outbox");

        return Task.CompletedTask;
    }

    public async Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = _inMemoryOutboxStore.Events.Where(x => x.ProcessedOn == null).ToList();

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
                    domainNotificationEvent?.Id);
            }

            if (outboxMessage.EventType == EventType.IntegrationEvent)
            {
                var integrationEvent = data as IIntegrationEvent;

                // integration event
                await _busPublisher.PublishAsync(integrationEvent, cancellationToken);

                _logger.LogInformation(
                    "Published a message: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    integrationEvent?.Id);
            }

            outboxMessage.MarkAsProcessed();
        }
    }
}
