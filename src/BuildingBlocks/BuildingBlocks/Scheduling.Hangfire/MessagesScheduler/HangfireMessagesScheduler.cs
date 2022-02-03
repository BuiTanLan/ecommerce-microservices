using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Scheduling;
using Hangfire;
using Newtonsoft.Json;

namespace BuildingBlocks.Scheduling.Hangfire.MessagesScheduler
{
    public class HangfireMessagesScheduler : IHangfireMessagesScheduler
    {
        private readonly IMessagesExecutor _messagesExecutor;
        private readonly ICommandProcessor _commandProcessor;

        public HangfireMessagesScheduler(IMessagesExecutor messagesExecutor, ICommandProcessor commandProcessor)
        {
            _messagesExecutor = messagesExecutor;
            _commandProcessor = commandProcessor;
        }

        public Task Enqueue<T>(T command, string description = null)
            where T : ICommand
        {
            _commandProcessor.Enqueue(command, description);

            return Task.CompletedTask;
        }

        public Task Enqueue(MessageSerializedObject messageSerializedObject, string description = null)
        {
            return Task.FromResult(
                BackgroundJob.Enqueue(() => _messagesExecutor.ExecuteCommand(messageSerializedObject)));
        }

        public string Enqueue<T>(
            T command,
            string parentJobId,
            JobContinuationOptions continuationOption,
            string description = null)
            where T : ICommand
        {
            var messageSerializedObject = SerializeObject(command, description);

            return BackgroundJob.ContinueJobWith(
                parentJobId,
                () => _messagesExecutor.ExecuteCommand(messageSerializedObject),
                continuationOption);
        }

        public string Enqueue(
            MessageSerializedObject messageSerializedObject,
            string parentJobId,
            JobContinuationOptions continuationOption,
            string description = null)
        {
            return BackgroundJob.ContinueJobWith(
                parentJobId,
                () => _messagesExecutor.ExecuteCommand(messageSerializedObject),
                continuationOption);
        }

        public Task Schedule<T>(T command, DateTimeOffset scheduleAt, string description = null)
            where T : ICommand
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);

            return Task.CompletedTask;
        }

        public Task Schedule(
            MessageSerializedObject messageSerializedObject,
            DateTimeOffset scheduleAt,
            string description = null)
        {
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(messageSerializedObject), scheduleAt);

            return Task.CompletedTask;
        }

        public Task Schedule<T>(T command, TimeSpan delay, string description = null)
            where T : ICommand
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), newTime);

            return Task.CompletedTask;
        }

        public Task Schedule(
            MessageSerializedObject messageSerializedObject,
            TimeSpan delay,
            string description = null)
        {
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(messageSerializedObject), newTime);

            return Task.CompletedTask;
        }

        public Task ScheduleRecurring<T>(T command, string name, string cronExpression, string description = null)
            where T : ICommand
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            RecurringJob.AddOrUpdate(name, () => _messagesExecutor.ExecuteCommand(mediatorSerializedObject),
                cronExpression, TimeZoneInfo.Local);

            return Task.CompletedTask;
        }

        public Task ScheduleRecurring(
            MessageSerializedObject messageSerializedObject,
            string name,
            string cronExpression,
            string description = null)
        {
            RecurringJob.AddOrUpdate(name, () => _messagesExecutor.ExecuteCommand(messageSerializedObject),
                cronExpression, TimeZoneInfo.Local);

            return Task.CompletedTask;
        }

        private MessageSerializedObject SerializeObject(object messageObject, string description)
        {
            string fullTypeName = messageObject.GetType().FullName;
            string data = JsonConvert.SerializeObject(messageObject, new JsonSerializerSettings
            {
                Formatting = Formatting.None,
            });

            return new MessageSerializedObject(fullTypeName, data, description,
                messageObject.GetType().Assembly.GetName().FullName);
        }
    }
}
