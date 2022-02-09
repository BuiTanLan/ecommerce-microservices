using BuildingBlocks.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Mongo;

/// <summary>
/// Defines the interface(s) for generic unit of work.
/// </summary>
public interface IUnitOfWork<out TContext> : IUnitOfWork
    where TContext : DbContext
{
    /// <summary>
    /// Gets the database context.
    /// </summary>
    /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
    TContext DbContext { get; }

    Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
