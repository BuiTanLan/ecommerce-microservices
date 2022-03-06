namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// The unit of work pattern.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task ExecuteAsync(Func<Task> action);
}

public interface IUnitOfWork<out TContext> : IDisposable
    where TContext : class
{
    TContext Context { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task ExecuteAsync(Func<Task> action);
}
