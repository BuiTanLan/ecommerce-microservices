using BuildingBlocks.Abstractions.Domain.Model;

namespace BuildingBlocks.Abstractions.Persistence.Mongo;

public interface IMongoEventSourcedRepository<TEntity> : IEventSourcedRepository<TEntity>
    where TEntity : class, IAggregate<Guid>, IHaveIdentity<Guid>, new()
{
}
