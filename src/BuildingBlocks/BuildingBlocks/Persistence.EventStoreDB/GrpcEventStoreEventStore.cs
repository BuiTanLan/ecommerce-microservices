using System.Text;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Persistence.EventStoreDB.Serialization;
using EventStore.Client;
using Newtonsoft.Json;

namespace BuildingBlocks.Persistence.EventStoreDB;

//https://developers.eventstore.com/clients/dotnet/5.0/migration-to-gRPC.html#update-the-target-framework
//https://www.youtube.com/watch?v=-4_KTfVkjlQ
// https://github.com/Eventuous/eventuous/blob/dev/src/EventStore/src/Eventuous.EventStore/EsdbEventStore.cs

public class GrpcEventStoreEventStore : IEventStore
{
    private readonly IEventStoreConnection _connection;
    private readonly EventStoreClient _eventStoreClient;

    public EventStoreEventStore(IEventStoreConnection connection, EventStoreClient eventStoreClient)
    {
        _connection = connection;
        _eventStoreClient = eventStoreClient;
    }

    public async Task AppendAsync<TAggregate>(
        string streamId,
        int? version,
        CancellationToken cancellationToken,
        params IDomainEvent[] events)
    {
        // // Using IEventStoreConnection
        // var eventsToStore = events
        //     .Select(EventStoreDbSerializer.ToJsonEventAPIData).ToArray();
        //
        // await _connection.AppendToStreamAsync(
        //     streamId.ToString(),
        //     version ?? ExpectedVersion.NoStream,
        //     eventsToStore);

        // Or Using EventStoreClient
        var eventsToStore = events
            .Select(EventStoreDbSerializer.ToJsonEventData).ToArray();

        // StreamNameMapper.ToStreamId<TAggregate>(aggregate.Id)
        await _eventStoreClient.AppendToStreamAsync(
            streamId,
            StreamState.Any,
            eventsToStore,
            cancellationToken: cancellationToken
        );
    }

    public async Task<TAggregate?> AggregateAsync<TAggregate>(
        string streamId, CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null)
        where TAggregate : class, new()
    {
        var readResult = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamId,
            EventStore.Client.StreamPosition.FromInt64(version),
            cancellationToken: cancellationToken
        );

        return await readResult.Select(@event => @event.Deserialize<TAggregate>())
            .AggregateAsync(default, cancellationToken);
    }

    public Task<IReadOnlyList<IDomainEvent>> QueryAsync(string? streamId = null,
        CancellationToken cancellationToken = default, int? fromVersion = null,
        DateTime? fromTimestamp = null)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Event>> QueryAsync(
        string streamId,
        CancellationToken cancellationToken = default,
        int version = 0)
    {
        var readResult = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamId.ToString(),
            EventStore.Client.StreamPosition.FromInt64(version),
            cancellationToken: cancellationToken
        ).Select(resolvedEvent => new Event(Deserialize(resolvedEvent.Event.EventType, resolvedEvent.Event.Data),
            resolvedEvent.Event.EventNumber.ToInt64()));

        return await readResult.ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEvent>> QueryAsync<TEvent>(string? streamId = null,
        CancellationToken cancellationToken = default, int? fromVersion = null,
        DateTime? fromTimestamp = null) where TEvent : class, IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private IDomainEvent Deserialize(string eventType, ReadOnlyMemory<byte> data)
    {
        return (IDomainEvent)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data.ToArray()),
            Type.GetType(eventType));
    }
}
