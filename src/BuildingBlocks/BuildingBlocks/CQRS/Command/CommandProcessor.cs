using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public class CommandProcessor : ICommandProcessor
{
    private readonly IMediator _mediator;

    public CommandProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }
    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
}
