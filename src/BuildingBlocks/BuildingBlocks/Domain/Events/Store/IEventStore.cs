namespace BuildingBlocks.Domain.Events.Store;

public interface IEventStore
{
    Task AppendAsync(Guid streamId, int? version, CancellationToken cancellationToken, params IEvent[] events);

    Task<TEntity> AggregateAsync<TEntity>(
        Guid streamId,
        CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null)
        where TEntity : class, new();

    Task<IReadOnlyList<IEvent>> QueryAsync(
        Guid? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null
    );

    Task<IReadOnlyList<TEvent>> QueryAsync<TEvent>(
        Guid? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null
    )
        where TEvent : class, IEvent;

    Task SaveChangesAsync(CancellationToken token = default);
}
