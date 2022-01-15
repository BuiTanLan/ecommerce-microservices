using System.Data;
using BuildingBlocks.Domain.Model;

namespace BuildingBlocks.EFCore;

/// <summary>
/// The unit of work pattern.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Begins an asynchronous transaction.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task BeginTransactionAsync(IsolationLevel isolationLevel);

    /// <summary>
    /// Commits an asynchronous transaction.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rollbacks a transaction.
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// Gets the specified repository for the <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">Type of key in the entity.</typeparam>
    /// <returns>An instance of type inherited from <see cref="IRepository{TEntity,TKey}"/> interface.</returns>
    public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : class, IAggregateRoot<TKey>;

    /// <summary>
    /// Creates an execution strategy.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RetryOnExceptionAsync(Func<Task> operation);
}
