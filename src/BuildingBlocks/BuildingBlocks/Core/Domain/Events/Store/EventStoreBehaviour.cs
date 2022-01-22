using BuildingBlocks.Core.Domain.Events.Internal;

namespace BuildingBlocks.Core.Domain.Events.Store;

public class EventStoreBehaviour<TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    private readonly IEventStore _eventStore;

    public EventStoreBehaviour(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(TEvent @event, CancellationToken cancellationToken)
    {
        await _eventStore.Append(@event.EventId, cancellationToken, @event);
        await _eventStore.SaveChangesAsync(cancellationToken);
    }
}
