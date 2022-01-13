using BuildingBlocks.Domain;
using BuildingBlocks.EFCore.Specification;

namespace BuildingBlocks.EFCore;

public interface IRepository<TEntity, TKey> : IDisposable
    where TEntity : IAggregateRoot<TKey>
{
    Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TEntity> FindOneAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<List<TEntity>> FindAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);
    Task<TEntity> EditAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);
}

public interface IRepository<TEntity> : IRepository<TEntity, Guid>
    where TEntity : IAggregateRoot<Guid>
{
}
