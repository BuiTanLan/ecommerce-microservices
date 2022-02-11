

using BuildingBlocks.CQRS.Command;

namespace BuildingBlocks.Messaging.Scheduling
{
    public interface IEnqueueMessages
    {
        Task EnqueueAsync<T>(T command, string description = null)
            where T : IInternalCommand;
        Task EnqueueAsync(MessageSerializedObject messageSerializedObject, string description = null);
    }
}
