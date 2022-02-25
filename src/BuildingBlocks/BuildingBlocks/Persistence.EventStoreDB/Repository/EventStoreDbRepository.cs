using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Domain.Events;
using BuildingBlocks.Abstractions.Domain.Model;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Persistence.EventStoreDB.Events;
using BuildingBlocks.Persistence.EventStoreDB.Serialization;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB.Repository;

// http://www.andreavallotti.tech/en/2018/01/event-sourcing-and-cqrs-in-c/
public class EventStoreDbRepository<TAggregate> : IEventSourcedRepository<TAggregate>
    where TAggregate : class, IHaveIdentity<Guid>, IAggregate<Guid>, new()
{
    private readonly EventStoreClient _eventStoreDbClient;
    private readonly IEventProcessor _eventProcessor;

    public EventStoreDbRepository(
        EventStoreClient eventStoreDbClient,
        IEventProcessor eventProcessor
    )
    {
        _eventStoreDbClient = eventStoreDbClient;
        _eventProcessor = eventProcessor;
    }

    public Task<TAggregate?> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        return _eventStoreDbClient.AggregateStream<TAggregate>(id, cancellationToken);
    }

    public Task<TAggregate> Add(TAggregate aggregate, CancellationToken cancellationToken)
    {
        return StoreAsync(aggregate, cancellationToken);
    }

    public Task<TAggregate> Update(
        TAggregate aggregate,
        int? expectedVersion,
        CancellationToken cancellationToken = default)
    {
        return StoreAsync(aggregate, cancellationToken);
    }

    public Task Delete(TAggregate aggregate, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        return StoreAsync(aggregate, cancellationToken);
    }

    public async Task DeleteById(Guid id, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        var aggregate = await _eventStoreDbClient.AggregateStream<TAggregate>(id, cancellationToken);
        if (aggregate == null)
        {
            return;
        }

        await Delete(aggregate, expectedVersion, cancellationToken);
    }

    private async Task<TAggregate> StoreAsync(TAggregate aggregate, CancellationToken cancellationToken)
    {
        Guard.Against.Null(aggregate, nameof(aggregate));

        var events = aggregate.GetUncommittedEvents();

        var eventsToStore = events
            .Select(EventStoreDbSerializer.ToJsonEventData).ToArray();

        await _eventStoreDbClient.AppendToStreamAsync(
            StreamNameMapper.ToStreamId<TAggregate>(aggregate.Id),
            StreamState.Any,
            eventsToStore,
            cancellationToken: cancellationToken
        );

        return aggregate;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
