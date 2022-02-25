using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Model;
using BuildingBlocks.Abstractions.Domain.Projections;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Persistence.EventStoreDB.Serialization;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB.Events;

// http://www.andreavallotti.tech/en/2018/01/event-sourcing-and-cqrs-in-c/
public static class AggregateStreamExtensions
{
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
