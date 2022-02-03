using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Scheduling.Helpers;
using Newtonsoft.Json;

namespace BuildingBlocks.Messaging.Scheduling
{
    public class MessagesExecutor : IMessagesExecutor
    {
        private readonly ICommandProcessor _commandProcessor;

        public MessagesExecutor(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public Task ExecuteCommand(MessageSerializedObject messageSerializedObject)
        {
            var type = messageSerializedObject.GetPayloadType();

            if (type != null)
            {
                dynamic req = JsonConvert.DeserializeObject(messageSerializedObject.Data, type);

                return _commandProcessor.SendAsync(req as ICommand);
            }

            return null;
        }
    }
}
