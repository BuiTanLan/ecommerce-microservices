using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Event.External;

public class EmptyExternalEventProducer : IExternalEventProducer
{
    public Task PublishAsync(IExternalEvent @event)
    {
        return Task.CompletedTask;
    }
}
