using BuildingBlocks.CQRS.Command;

namespace BuildingBlocks.Messaging.Scheduling
{
    public interface IMessagesScheduler : IEnqueueMessages
    {
        Task Schedule<T>(T command, DateTimeOffset scheduleAt, string description = null)
            where T : ICommand;

        Task Schedule(
            MessageSerializedObject messageSerializedObject,
            DateTimeOffset scheduleAt,
            string description = null);

        Task Schedule<T>(T command, TimeSpan delay, string description = null)
            where T : ICommand;
        Task Schedule(MessageSerializedObject messageSerializedObject, TimeSpan delay, string description = null);

        Task ScheduleRecurring<T>(T command, string name, string cronExpression, string description = null)
            where T : ICommand;

        Task ScheduleRecurring(
            MessageSerializedObject messageSerializedObject,
            string name,
            string cronExpression,
            string description = null);
    }
}
