using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Query;

public interface IQueryProcessor
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
