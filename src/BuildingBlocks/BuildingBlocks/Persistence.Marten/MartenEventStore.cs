using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using Marten;


namespace BuildingBlocks.Persistence.Marten;

public class MartenEventStore : IEventStore
{
    private readonly IDocumentSession _documentSession;

    public MartenEventStore(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public Task AppendAsync<TAggregate>(
        Guid streamId,
        int? version,
        CancellationToken cancellationToken = default,
        params IDomainEvent[] events)
    {
        if (version.HasValue)
            _documentSession.Events.Append(streamId, version, events.Cast<object>().ToArray());
        else
            _documentSession.Events.Append(streamId, events.Cast<object>().ToArray());
        return Task.CompletedTask;
    }

    public Task<TAggregate?> AggregateAsync<TAggregate>(
        Guid streamId,
        CancellationToken cancellationToken = default,
        int version = 0,
        DateTime? timestamp = null)
        where TAggregate : class, new()
    {
        return _documentSession.Events.AggregateStreamAsync<TAggregate>(
            streamId,
            version,
            timestamp,
            token: cancellationToken);
    }

    public async Task<IReadOnlyList<IDomainEvent>> QueryAsync(
        Guid? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null)
    {
        var events = await Filter(streamId, fromVersion, fromTimestamp)
            .ToListAsync(cancellationToken);

        return events
            .Select(ev => ev.Data)
            .OfType<IDomainEvent>()
            .ToList();
    }

    public Task<IEnumerable<Event>> QueryAsync(Guid streamId, CancellationToken cancellationToken = default, int version = 0)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<TEvent>> QueryAsync<TEvent>(
        Guid? streamId = null,
        CancellationToken cancellationToken = default,
        int? fromVersion = null,
        DateTime? fromTimestamp = null)
        where TEvent : class, IDomainEvent
    {
        var events = await Filter(streamId, fromVersion, fromTimestamp)
            .ToListAsync(cancellationToken);

        return events
            .Select(ev => ev.Data)
            .OfType<TEvent>()
            .ToImmutableList();
    }

    private IQueryable<global::Marten.Events.IEvent> Filter(Guid? streamId, int? version, DateTime? timestamp)
    {
        var query = _documentSession.Events.QueryAllRawEvents().AsQueryable();

        if (streamId.HasValue)
            query = query.Where(ev => ev.StreamId == streamId);

        if (version.HasValue)
            query = query.Where(ev => ev.Version >= version);

        if (timestamp.HasValue)
            query = query.Where(ev => ev.Timestamp >= timestamp);

        return query;
    }

    public Task SaveChangesAsync(CancellationToken token = default)
    {
        return _documentSession.SaveChangesAsync(token);
    }
}
