using System.ComponentModel;
using BuildingBlocks.CQRS.Command;

namespace BuildingBlocks.Scheduling.Hangfire
{
    public class CommandProcessorHangfireBridge
    {
        private readonly ICommandProcessor _commandProcessor;

        public CommandProcessorHangfireBridge(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [DisplayName("{1}")]
        public Task Send(ICommand command, string description = "")
        {
            return _commandProcessor.SendAsync(command);
        }

        [DisplayName("{0}")]
        public Task Send(string jobName, ICommand command)
        {
            return _commandProcessor.SendAsync(command);
        }
    }
}
