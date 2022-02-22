using BuildingBlocks.Abstractions.Domain.Model;

namespace BuildingBlocks.Abstractions.Persistence;

public interface IEventSourcedRepository<TAggregate> : IDisposable
    where TAggregate : class, IAggregate<Guid>, IHaveIdentity<Guid>, new()
{
    Task<TAggregate?> FindById(Guid id, CancellationToken cancellationToken = default);

    Task<TAggregate> Add(TAggregate aggregate, CancellationToken cancellationToken);

    Task<TAggregate> Update(TAggregate aggregate, int? expectedVersion, CancellationToken cancellationToken = default);

    Task Delete(TAggregate aggregate, int? expectedVersion, CancellationToken cancellationToken = default);

    Task DeleteById(Guid id, int? expectedVersion, CancellationToken cancellationToken = default);
}
