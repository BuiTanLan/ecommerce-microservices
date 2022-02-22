using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Events.Store;

namespace BuildingBlocks.Persistence.EventStoreDB;

public class EventStore : IEventStore
{
    public Task AppendAsync<TEntity>(Guid streamId, int? version, CancellationToken cancellationToken,
        params IDomainEvent[] events)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> AggregateAsync<TEntity>(Guid streamId, CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null) where TEntity : class, new()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<IDomainEvent>> QueryAsync(Guid? streamId = null,
        CancellationToken cancellationToken = default, int? fromVersion = null,
        DateTime? fromTimestamp = null)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TEvent>> QueryAsync<TEvent>(Guid? streamId = null,
        CancellationToken cancellationToken = default, int? fromVersion = null,
        DateTime? fromTimestamp = null) where TEvent : class, IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
