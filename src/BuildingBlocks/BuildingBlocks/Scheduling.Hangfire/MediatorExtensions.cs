using BuildingBlocks.CQRS.Command;
using Hangfire;

namespace BuildingBlocks.Scheduling.Hangfire
{
    public static class CommandProcessorExtensions
    {
        public static void Enqueue(this ICommandProcessor commandProcessor, string jobName, ICommand command)
        {
            var client = new BackgroundJobClient();
            client.Enqueue<CommandProcessorHangfireBridge>(bridge => bridge.Send(jobName, command));
        }

        public static void Enqueue(this ICommandProcessor commandProcessor, ICommand command, string description = null)
        {
            var client = new BackgroundJobClient();

            // https://codeopinion.com/using-hangfire-and-mediatr-as-a-message-dispatcher/
            // client.Enqueue<IMediator>(x => x.Send(request, default)); // we could use our mediator directly but because we want to use some hangfire attribute we will wap it in a bridge
            client.Enqueue<CommandProcessorHangfireBridge>(bridge => bridge.Send(command, description));
        }
    }
}
