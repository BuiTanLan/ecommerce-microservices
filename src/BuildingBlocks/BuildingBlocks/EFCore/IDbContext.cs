using System.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EFCore;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RetryOnExceptionAsync(Func<Task> operation);
    Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation);
}
