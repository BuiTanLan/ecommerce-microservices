using System.Linq.Expressions;
using BuildingBlocks.Domain.Model;
using BuildingBlocks.EFCore.Specification;

namespace BuildingBlocks.EFCore;

public interface IRepository<TEntity, TKey> : IDisposable
    where TEntity : IAggregateRoot<TKey>
{
    Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<TEntity> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TEntity> FindOneAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<IList<TEntity>> FindAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    Task<IList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> EditAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
}

public interface IRepository<TEntity> : IRepository<TEntity, Guid>
    where TEntity : IAggregateRoot<Guid>
{
}
