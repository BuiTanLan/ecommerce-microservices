using BuildingBlocks.Messaging.Outbox;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public class CommandProcessor : ICommandProcessor
{
    private readonly IMediator _mediator;
    private readonly IOutboxService _outboxService;
    // private readonly IMessageScheduler _messageScheduler;

    public CommandProcessor(
        IMediator mediator,
        IOutboxService outboxService
        // IMessageScheduler messageScheduler
    )
    {
        _mediator = mediator;
        _outboxService = outboxService;
        // _messageScheduler = messageScheduler;
    }

    public async Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        if (command is IInternalCommand internalCommand)
        {
            await _outboxService.SaveAsync(internalCommand, cancellationToken);

            // Or
            // await _messagesScheduler.Enqueue(new SendRestockNotification(notification.ProductId, notification.NewStock));
            return default!;
        }

        return await _mediator.Send(command, cancellationToken);
    }
}
