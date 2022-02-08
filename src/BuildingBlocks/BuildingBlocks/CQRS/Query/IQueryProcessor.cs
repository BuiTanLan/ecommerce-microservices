namespace BuildingBlocks.CQRS.Query;

public interface IQueryProcessor
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TResponse> SendAsync<TResponse>(IStreamQuery<TResponse> query, CancellationToken cancellationToken = default);
}
