namespace BuildingBlocks.EFCore;

public interface ITxDbContextExecutes
{
    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default);
    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
