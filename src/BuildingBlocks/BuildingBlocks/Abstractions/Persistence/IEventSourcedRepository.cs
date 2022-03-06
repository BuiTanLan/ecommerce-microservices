using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Model;

namespace BuildingBlocks.Abstractions.Persistence;

public interface IEventSourcedRepository<TAggregate> : IDisposable
    where TAggregate : class, IAggregate<Guid>, IHaveIdentity<Guid>, new()
{
    public Task<TAggregate?> Find(string streamId, Func<TAggregate?, IDomainEvent, TAggregate> when,
        CancellationToken cancellationToken = default);

    public Task<TAggregate?> Find(string streamId, CancellationToken cancellationToken = default);

    public Task Append(string streamId, IDomainEvent @event, CancellationToken cancellationToken = default);

    public Task Append(string streamId, IDomainEvent @event, int version,
        CancellationToken cancellationToken = default);
}
