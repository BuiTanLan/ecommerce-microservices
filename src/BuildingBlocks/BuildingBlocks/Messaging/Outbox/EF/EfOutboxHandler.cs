using BuildingBlocks.Domain.Events;
using BuildingBlocks.EFCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Outbox;

public class EfOutboxHandler : INotificationHandler<EventWrapper>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EfOutboxHandler> _logger;

    public EfOutboxHandler(IUnitOfWork unitOfWork, ILogger<EfOutboxHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(EventWrapper eventWrapper, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Store @event: {nameof(@eventWrapper.Event)} into the in-memory EventStore.");

        var outboxEntity = new OutboxMessage(Guid.NewGuid(), DateTime.Now, eventWrapper.Event);
        await _unitOfWork.GetRepository<OutboxMessage, Guid>()
            .AddAsync(outboxEntity, cancellationToken: cancellationToken);
    }
}
