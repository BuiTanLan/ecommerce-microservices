using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Scheduling.Helpers;
using MediatR;
using Newtonsoft.Json;

namespace BuildingBlocks.Messaging.Scheduling;

public class MessagesExecutor : IMessagesExecutor
{
    private readonly IMediator _mediator;

    public MessagesExecutor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task ExecuteCommand(MessageSerializedObject messageSerializedObject)
    {
        var type = messageSerializedObject.GetPayloadType();

        dynamic req = JsonConvert.DeserializeObject(messageSerializedObject.Data, type);

        return _mediator.Send(req as InternalCommand);
    }
}
