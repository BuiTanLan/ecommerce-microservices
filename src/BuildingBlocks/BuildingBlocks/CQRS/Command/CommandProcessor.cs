using BuildingBlocks.Messaging.Scheduling;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public class CommandProcessor : ICommandProcessor
{
    private readonly IMediator _mediator;
    private readonly IMessagesScheduler _messagesScheduler;

    public CommandProcessor(
        IMediator mediator,
        IMessagesScheduler messagesScheduler
    )
    {
        _mediator = mediator;
        _messagesScheduler = messagesScheduler;
    }

    public async Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        if (command is IInternalCommand internalCommand)
        {
            await _messagesScheduler.EnqueueAsync(internalCommand);

            return default!;
        }

        return await _mediator.Send(command, cancellationToken);
    }
}
