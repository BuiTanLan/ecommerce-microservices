using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Scheduling;
using Hangfire;

namespace BuildingBlocks.Scheduling.Hangfire.MessagesScheduler
{
    public interface IHangfireMessagesScheduler : IMessagesScheduler
    {
        string Enqueue<T>(
            T command,
            string parentJobId,
            JobContinuationOptions continuationOption,
            string description = null)
            where T : IInternalCommand;

        string Enqueue(
            MessageSerializedObject messageSerializedObject,
            string parentJobId,
            JobContinuationOptions continuationOption,
            string description = null);
    }
}
