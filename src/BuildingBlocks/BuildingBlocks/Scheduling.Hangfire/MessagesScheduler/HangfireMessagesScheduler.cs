using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Scheduling;
using Hangfire;
using Newtonsoft.Json;

namespace BuildingBlocks.Scheduling.Hangfire.MessagesScheduler
{
    public class HangfireMessagesScheduler : IHangfireMessagesScheduler
    {
        private readonly IMessagesExecutor _messagesExecutor;

        public HangfireMessagesScheduler(IMessagesExecutor messagesExecutor)
        {
            _messagesExecutor = messagesExecutor;
        }

        public Task EnqueueAsync<T>(T command, string description = null)
            where T : IInternalCommand
        {
            var client = new BackgroundJobClient();

            // https://codeopinion.com/using-hangfire-and-mediatr-as-a-message-dispatcher/
            // client.Enqueue<IMediator>(x => x.Send(request, default)); // we could use our mediator directly but because we want to use some hangfire attribute we will wap it in a bridge
            client.Enqueue<CommandProcessorHangfireBridge>(bridge => bridge.Send(command, description));

            return Task.CompletedTask;
        }

        public Task EnqueueAsync(MessageSerializedObject messageSerializedObject, string description = null)
        {
            return Task.FromResult(
                BackgroundJob.Enqueue(() => _messagesExecutor.ExecuteCommand(messageSerializedObject)));
        }

        public string Enqueue<T>(
            T command,
            string parentJobId,
            JobContinuationOptions continuationOption,
            string description = null)
            where T : IInternalCommand
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

        public Task ScheduleAsync<T>(T command, DateTimeOffset scheduleAt, string description = null)
            where T : IInternalCommand
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);

            return Task.CompletedTask;
        }

        public Task ScheduleAsync(
            MessageSerializedObject messageSerializedObject,
            DateTimeOffset scheduleAt,
            string description = null)
        {
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(messageSerializedObject), scheduleAt);

            return Task.CompletedTask;
        }

        public Task ScheduleAsync<T>(T command, TimeSpan delay, string description = null)
            where T : IInternalCommand
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), newTime);

            return Task.CompletedTask;
        }

        public Task ScheduleAsync(
            MessageSerializedObject messageSerializedObject,
            TimeSpan delay,
            string description = null)
        {
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(messageSerializedObject), newTime);

            return Task.CompletedTask;
        }

        public Task ScheduleRecurringAsync<T>(T command, string name, string cronExpression, string description = null)
            where T : IInternalCommand
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            RecurringJob.AddOrUpdate(name, () => _messagesExecutor.ExecuteCommand(mediatorSerializedObject),
                cronExpression, TimeZoneInfo.Local);

            return Task.CompletedTask;
        }

        public Task ScheduleRecurringAsync(
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
            string data = JsonConvert.SerializeObject(messageObject,
                new JsonSerializerSettings { Formatting = Formatting.None, });

            return new MessageSerializedObject(fullTypeName, data, description,
                messageObject.GetType().Assembly.GetName().FullName);
        }
    }
}
