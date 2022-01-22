namespace BuildingBlocks.Core.Domain.Events.Store;

public static class EventStoreExtensions
{
    public static Task Append(this IEventStore eventStore, Guid streamId, CancellationToken cancellationToken, params IEvent[] events)
    {
        return eventStore.AppendAsync(streamId, null, cancellationToken, events);
    }
}
