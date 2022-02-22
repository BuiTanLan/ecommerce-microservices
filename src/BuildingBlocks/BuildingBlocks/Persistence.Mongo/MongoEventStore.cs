using Baseline;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Events.Store;
using BuildingBlocks.Abstractions.Domain.Model;
using BuildingBlocks.Core.Utils.Reflections;
using MongoDB.Driver;
using Newtonsoft.Json;
using Stream = BuildingBlocks.Abstractions.Domain.Events.Store.Stream;

namespace BuildingBlocks.Persistence.Mongo;

// https://cqrs.wordpress.com/documents/building-event-storage/
public class MongoEventStore : IEventStore
{
    private readonly IMongoDbContext _mongoDbContext;

    private readonly IMongoCollection<Stream> _streamEvent;

    public MongoEventStore(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
        _streamEvent = _mongoDbContext.GetCollection<Stream>("streams");
    }

    public async Task AppendAsync<TEntity>(
        Guid streamId,
        int? version,
        CancellationToken cancellationToken,
        params IDomainEvent[] events)
    {
        var stream = await _streamEvent.Find(x => x.Id == streamId).FirstOrDefaultAsync(cancellationToken);
        if (stream is { })
        {
            var streamEvents = events.Select(domainEvent => new StreamEvent
            {
                Data = JsonConvert.SerializeObject(domainEvent),
                StreamId = streamId,
                Version = version ?? 0,
                Id = Guid.NewGuid(),
                EventTypeName = domainEvent.GetType().AssemblyQualifiedName
            }).ToList();

            stream.Events.AddRange(streamEvents);

            stream.Version = stream.Version + streamEvents.Count;

            _mongoDbContext.AddCommand(
                async () => await _streamEvent.ReplaceOneAsync(
                    x => x.Id == streamId,
                    stream,
                    cancellationToken: cancellationToken));
        }
        else
        {
            var streamEvents = events.Select(domainEvent => new StreamEvent
            {
                Data = JsonConvert.SerializeObject(domainEvent),
                StreamId = streamId,
                Version = version ?? 0,
                Id = Guid.NewGuid(),
                EventTypeName = domainEvent.GetType().AssemblyQualifiedName
            }).ToList();

            stream = new Stream
            {
                Id = streamId, Type = typeof(TEntity).Name, Events = streamEvents, Version = streamEvents.Count
            };

            _mongoDbContext.AddCommand(
                async () => await _streamEvent.InsertOneAsync(stream, cancellationToken: cancellationToken));

            await _streamEvent.InsertOneAsync(stream, cancellationToken: cancellationToken);
        }
    }

    public async Task<TEntity?> AggregateAsync<TEntity>(
        Guid streamId,
        CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null)
        where TEntity : class, new()
    {
        var aggregate = (TEntity)Activator.CreateInstance(typeof(TEntity), true)!;

        var events = await QueryAsync(streamId, cancellationToken, version, timestamp);

        foreach (var @event in events)
        {
            aggregate.InvokeIfExists("Apply", @event);
            aggregate.SetIfExists(nameof(IAggregate.Version), ++version);
        }

        return aggregate;
    }

    public async Task<IReadOnlyList<IDomainEvent>> QueryAsync(
        Guid? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null)
    {
        var streams = await _streamEvent.Find(x => x.Id == streamId || streamId == null)
            .ToListAsync(cancellationToken: cancellationToken);

        if (streams is null)
        {
            return new List<IDomainEvent>();
        }

        var query = streams.SelectMany(x => x.Events)
            .Where(x => x.Version >= fromVersion).OrderBy(x => x.Version)
            .Select(x => (dynamic)x);

        var result = query.Select(se =>
                JsonConvert.DeserializeObject(se.Data, Type.GetType(se.EventTypeName)))
            .OfType<IDomainEvent>()
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<TEvent>> QueryAsync<TEvent>(
        Guid? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null)
        where TEvent : class, IDomainEvent
    {
        var streams = await _streamEvent.Find(x => x.Id == streamId || streamId == null)
            .ToListAsync(cancellationToken: cancellationToken);

        if (streams is null)
        {
            return new List<TEvent>();
        }

        var query = streams.SelectMany(x => x.Events)
            .Where(x => x.Version >= fromVersion).OrderBy(x => x.Version)
            .Select(x => (dynamic)x);

        var result = query.Select(se =>
                JsonConvert.DeserializeObject(se.Data, Type.GetType(se.EventTypeName)))
            .OfType<TEvent>()
            .ToList();

        return result;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}
