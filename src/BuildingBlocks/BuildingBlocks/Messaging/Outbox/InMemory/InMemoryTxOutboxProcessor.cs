using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Outbox.InMemory;

public class InMemoryTxOutboxProcessor : ITxOutboxProcessor
{
    private readonly IBusPublisher _busPublisher;
    private readonly IEventStorage _eventStorage;
    private readonly ILogger<InMemoryTxOutboxProcessor> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMediator _mediator;

    public InMemoryTxOutboxProcessor(
        IBusPublisher busPublisher,
        IEventStorage eventStorage,
        ILogger<InMemoryTxOutboxProcessor> logger,
        IMessageSerializer messageSerializer,
        IMediator mediator)
    {
        _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
        _eventStorage = eventStorage ?? throw new ArgumentNullException(nameof(eventStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(Type integrationAssemblyType, CancellationToken cancellationToken = default)
    {
        var unsentMessages = _eventStorage.Events
            .Where(x => x.ProcessedOn == null).ToList();

        if (!unsentMessages.Any())
        {
            _logger.LogTrace("No unsent messages found in the outbox");
            return;
        }

        foreach (var outboxMessage in unsentMessages)
        {
            var type = Type.GetType(outboxMessage.Type);

            var data = _messageSerializer.Deserialize(outboxMessage.Data, type);
            if (data is null)
            {
                _logger.LogError("Invalid message type: {Name}", type?.Name);
                continue;
            }

            if (data is IDomainEvent domainEvent)
            {
                // domain event notification
                await _mediator.Publish(domainEvent, cancellationToken);

                _logger.LogInformation(
                    "Published a domain event : '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    domainEvent.Id);
            }

            if (data is IIntegrationEvent integrationEvent)
            {
                // integration event
                await _busPublisher.PublishAsync(integrationEvent, token: cancellationToken);

                _logger.LogInformation(
                    "Published a integration event: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    integrationEvent.Id);
            }

            outboxMessage.ChangeProcessDate();
        }
    }
}
