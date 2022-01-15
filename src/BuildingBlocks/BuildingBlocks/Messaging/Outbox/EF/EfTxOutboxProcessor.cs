using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.EFCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Outbox;

public class EfTxOutboxProcessor : ITxOutboxProcessor
{
    private readonly IBusPublisher _busPublisher;
    private readonly ILogger<EfTxOutboxProcessor> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public EfTxOutboxProcessor(
        IBusPublisher busPublisher,
        ILogger<EfTxOutboxProcessor> logger,
        IMessageSerializer messageSerializer,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(Type integrationAssemblyType, CancellationToken cancellationToken = default)
    {
        var unsentMessages = await _unitOfWork.GetRepository<OutboxMessage, Guid>()
            .FindAsync(x => x.ProcessedOn == null, cancellationToken);

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
