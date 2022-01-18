using BuildingBlocks.Domain.Model;

namespace BuildingBlocks.EFCore;

/// <summary>
/// The unit of work pattern.
/// </summary>
public interface IUnitOfWork : IDbContext, IDisposable
{
    /// <summary>
    /// Gets the specified repository for the <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">Type of key in the entity.</typeparam>
    /// <returns>An instance of type inherited from <see cref="IRepository{TEntity,TKey}"/> interface.</returns>
    public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : class, IAggregateRoot<TKey>;

    public IRepository<TEntity, Guid> GetRepository<TEntity>()
        where TEntity : class, IAggregateRoot;
}
