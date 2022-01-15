using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Outbox.InMemory;

public class InMemoryOutboxHandler : INotificationHandler<EventWrapper>
{
    private readonly IEventStorage _eventStorage;
    private readonly ILogger<InMemoryOutboxHandler> _logger;

    public InMemoryOutboxHandler(IEventStorage eventStorage, ILogger<InMemoryOutboxHandler> logger)
    {
        _eventStorage = eventStorage ?? throw new ArgumentNullException(nameof(eventStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(EventWrapper eventWrapper, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Store @event: {nameof(@eventWrapper.Event)} into the in-memory EventStore.");

        var outboxEntity = new OutboxMessage(Guid.NewGuid(), DateTime.Now, eventWrapper.Event);
        _eventStorage.Events.Add(outboxEntity);

        return Task.CompletedTask;
    }
}
