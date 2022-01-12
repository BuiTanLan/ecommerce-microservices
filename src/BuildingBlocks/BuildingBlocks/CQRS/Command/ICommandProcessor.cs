using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Command;

public interface ICommandProcessor
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}
