using BuildingBlocks.Abstractions.Domain.Events.Internal;

namespace BuildingBlocks.Abstractions.Persistence.EventStore;

public interface IEventStore
{
    Task AppendAsync<TEntity>(
        string streamId,
        int? version,
        CancellationToken cancellationToken,
        params IDomainEvent[] events);

    Task<TAggregate?> AggregateAsync<TAggregate>(
        Guid streamId,
        CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null)
        where TAggregate : class, new();

    Task<IReadOnlyList<IDomainEvent>> QueryAsync(
        string? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null
    );

    Task<IEnumerable<Event>> QueryAsync(
        string streamId,
        CancellationToken cancellationToken = default,
        int version = 0);

    Task<IReadOnlyList<TEvent>> QueryAsync<TEvent>(
        string? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null
    )
        where TEvent : class, IDomainEvent;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
