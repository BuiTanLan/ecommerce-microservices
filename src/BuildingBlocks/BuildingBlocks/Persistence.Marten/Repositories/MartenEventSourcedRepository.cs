using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Domain.Events;
using BuildingBlocks.Abstractions.Domain.Model;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Abstractions.Persistence.Marten;
using Marten;

namespace BuildingBlocks.Persistence.Marten.Repositories;

public class MartenEventSourcedRepository<TEntity> : IMartenEventSourcedRepository<TEntity>
    where TEntity : class, IAggregate<Guid>, IHaveIdentity<Guid>, new()
{
    private readonly IAggregatesDomainEventsStore _aggregatesDomainEventsStore;
    private readonly IDocumentSession _documentSession;
    private readonly IEventStore _eventStore;

    public MartenEventSourcedRepository(
        IDocumentSession documentSession,
        MartenEventStore eventStore,
        IAggregatesDomainEventsStore aggregatesDomainEventsStore)
    {
        _aggregatesDomainEventsStore = aggregatesDomainEventsStore;
        _documentSession = Guard.Against.Null(documentSession, nameof(documentSession));
        _eventStore = Guard.Against.Null(eventStore, nameof(eventStore));
    }

    public async Task<TEntity?> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        return (await _documentSession.Events.FetchStreamStateAsync(id, cancellationToken)) != null
            ? await _eventStore.AggregateAsync<TEntity>(id, cancellationToken)
            : null;
    }

    public Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken)
    {
        return StoreAsync(entity, null, cancellationToken);
    }

    public Task<TEntity> Update(TEntity entity, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        return StoreAsync(entity, null, cancellationToken);
    }

    public Task Delete(TEntity entity, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        return StoreAsync(entity, expectedVersion, cancellationToken);
    }

    public async Task DeleteById(Guid id, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        var entity = await FindById(id, cancellationToken);
        Guard.Against.NotFound(nameof(entity.Id), entity, nameof(entity.Id));

        await StoreAsync(entity, expectedVersion, cancellationToken);
    }

    private async Task<TEntity> StoreAsync(
        TEntity entity,
        int? expectedVersion,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(entity, nameof(entity));

        var entityEvents = _aggregatesDomainEventsStore.AddEventsFrom(entity).ToArray();

        await _eventStore.AppendAsync<TEntity>(entity.Id, expectedVersion, cancellationToken, entityEvents);

        return entity;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
