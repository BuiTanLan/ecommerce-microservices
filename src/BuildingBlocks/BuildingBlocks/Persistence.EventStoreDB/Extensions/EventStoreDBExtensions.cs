using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Model;
using BuildingBlocks.Abstractions.Domain.Projections;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Persistence.EventStoreDB.Serialization;
using EventStore.Client;
using Marten.Events.Projections;

namespace ECommerce.Core.EventStoreDB;

public static class EventStoreDBExtensions
{
    public static async Task Append(
        this EventStoreClient eventStore,
        string id,
        object @event,
        CancellationToken cancellationToken
    )
    {
        // for new stream with specific Id we expected no stream in the event-store
        await eventStore.AppendToStreamAsync(
            id,
            StreamState.NoStream,
            new[] { @event.ToJsonEventData() },
            cancellationToken: cancellationToken
        );
    }


    public static async Task Append(
        this EventStoreClient eventStore,
        string id,
        object @event,
        int version,
        CancellationToken cancellationToken
    )
    {
        // with version have optimistic concurrency
        await eventStore.AppendToStreamAsync(
            id,
            StreamRevision.FromInt64(version),
            new[] { @event.ToJsonEventData() },
            cancellationToken: cancellationToken
        );
    }

    public static async Task<TEntity?> AggregateStream<TEntity>(
        this EventStoreClient eventStore,
        Func<TEntity?>? getDefault,
        Func<TEntity?, object, TEntity> when,
        string id,
        CancellationToken cancellationToken)
    {
        var readResult = eventStore.ReadStreamAsync(
            Direction.Forwards,
            id,
            StreamPosition.Start,
            cancellationToken: cancellationToken
        );

        // Using LINQ aggregate method for aggregating events with `when` aggregate function
        return (await readResult
            .Select(@event => @event.Deserialize())
            .AggregateAsync(
                getDefault!(),
                when,
                cancellationToken
            ))!;
    }

    public static async Task<T?> AggregateStream<T>(
        this EventStoreClient eventStore,
        Guid id,
        CancellationToken cancellationToken,
        ulong? fromVersion = null)
        where T : class, IHaveProjection, IHaveEventSourcedAggregate
    {
        await using var readResult = eventStore.ReadStreamAsync(
            Direction.Forwards,
            StreamNameMapper.ToStreamId<T>(id),
            fromVersion ?? StreamPosition.Start,
            cancellationToken: cancellationToken
        );

        var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

        // foreach (var @event in await eventStore.ReadEventsAsync(id))
        // {
        //     aggregate.ApplyEvent(@event.DomainEvent, @event.EventNumber);
        // }

        await foreach (var @event in readResult)
        {
            var eventData = @event.Deserialize();

            aggregate.When(eventData!);
            aggregate.ApplyEvent((eventData as IDomainEvent)!, 0);
        }

        return aggregate;
    }
}
