using BuildingBlocks.CQRS.Command;

namespace BuildingBlocks.Messaging.Scheduling
{
    public interface IMessagesScheduler : IEnqueueMessages
    {
        Task ScheduleAsync<T>(T command, DateTimeOffset scheduleAt, string description = null)
            where T : IInternalCommand;

        Task ScheduleAsync(
            MessageSerializedObject messageSerializedObject,
            DateTimeOffset scheduleAt,
            string description = null);

        Task ScheduleAsync<T>(T command, TimeSpan delay, string description = null)
            where T : IInternalCommand;
        Task ScheduleAsync(MessageSerializedObject messageSerializedObject, TimeSpan delay, string description = null);

        Task ScheduleRecurringAsync<T>(T command, string name, string cronExpression, string description = null)
            where T : IInternalCommand;

        Task ScheduleRecurringAsync(
            MessageSerializedObject messageSerializedObject,
            string name,
            string cronExpression,
            string description = null);
    }
}
