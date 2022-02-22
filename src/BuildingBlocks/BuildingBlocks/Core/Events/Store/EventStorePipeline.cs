using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Domain.Events;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Events.Store;

namespace BuildingBlocks.Core.Events.Store;

public class EventStorePipeline<TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    private readonly IEventStore _eventStore;

    public EventStorePipeline(IEventStore eventStore)
    {
        _eventStore = Guard.Against.Null(eventStore, nameof(eventStore));
    }

    public async Task Handle(TEvent @event, CancellationToken cancellationToken)
    {
        // await _eventStore.AppendAsync<TEvent>();
        // await _eventStore.SaveChangesAsync(cancellationToken);
    }
}
