using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Persistence;

/// <summary>
/// The unit of work pattern.
/// </summary>
public interface IUnitOfWork : ITxDbContextExecutes, IDisposable
{
    /// <summary>
    /// Gets the specified repository for the <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">Type of key in the entity.</typeparam>
    /// <returns>An instance of type inherited from <see cref="IRepository{TEntity,TKey}"/> interface.</returns>
    public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : class, IAggregateRoot<TKey>;

    public IRepository<TEntity, long> GetRepository<TEntity>()
        where TEntity : class, IAggregateRoot<long>;

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
