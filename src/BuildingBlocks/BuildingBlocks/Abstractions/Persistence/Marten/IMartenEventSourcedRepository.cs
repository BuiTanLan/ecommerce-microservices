using BuildingBlocks.Abstractions.Domain.Model;

namespace BuildingBlocks.Abstractions.Persistence.Marten;

public interface IMartenEventSourcedRepository<TEntity> : IEventSourcedRepository<TEntity>
    where TEntity : class, IAggregate<Guid>, IHaveIdentity<Guid>, new()
{
}
