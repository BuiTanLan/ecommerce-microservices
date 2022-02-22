using BuildingBlocks.Abstractions.Domain.Events.Internal;

namespace BuildingBlocks.Abstractions.Domain.Events.Store;

public interface IEventStore
{
    Task AppendAsync<TEntity>(
        Guid streamId,
        int? version,
        CancellationToken cancellationToken,
        params IDomainEvent[] events);

    Task<TEntity?> AggregateAsync<TEntity>(
        Guid streamId,
        CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null)
        where TEntity : class, new();

    Task<IReadOnlyList<IDomainEvent>> QueryAsync(
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
        where TEvent : class, IDomainEvent;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
