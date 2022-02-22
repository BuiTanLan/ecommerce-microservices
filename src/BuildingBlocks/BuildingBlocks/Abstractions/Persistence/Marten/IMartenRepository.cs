using BuildingBlocks.Abstractions.Domain.Model;

namespace BuildingBlocks.Abstractions.Persistence.Marten;

public interface IMartenRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IHaveIdentity<TKey>
{
}
