using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.Abstractions.Scheduler;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging;
using Hangfire;

namespace BuildingBlocks.Scheduling.Hangfire.MessagesScheduler;

public interface IHangfireScheduler : IScheduler
{
    string Enqueue<T>(
        T command,
        string parentJobId,
        JobContinuationOptions continuationOption,
        string? description = null)
        where T : IInternalCommand;

    string Enqueue(
        ScheduleSerializedObject scheduleSerializedObject,
        string parentJobId,
        JobContinuationOptions continuationOption,
        string? description = null);
}
