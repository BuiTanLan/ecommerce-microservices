using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Event.External;

public interface IExternalEventProducer
{
    Task PublishAsync(IExternalEvent @event);
}
