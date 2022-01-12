using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BuildingBlocks.CQRS.Query;

public class QueryProcessor : IQueryProcessor
{
    private readonly IMediator _mediator;

    public QueryProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }
}
