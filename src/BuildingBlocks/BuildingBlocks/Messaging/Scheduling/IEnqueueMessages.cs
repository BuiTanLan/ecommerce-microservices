

using BuildingBlocks.CQRS.Command;

namespace BuildingBlocks.Messaging.Scheduling
{
    public interface IEnqueueMessages
    {
        Task Enqueue<T>(T command, string description = null)
            where T : ICommand;
        Task Enqueue(MessageSerializedObject messageSerializedObject, string description = null);
    }
}
