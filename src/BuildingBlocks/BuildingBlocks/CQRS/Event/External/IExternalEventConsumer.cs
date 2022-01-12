using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Event.External;

public interface IExternalEventConsumer
{
    Task StartAsync(CancellationToken cancellationToken);
}
