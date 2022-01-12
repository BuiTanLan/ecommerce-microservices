using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Event;

public interface IEventProcessor
{
    Task PublishAsync(params IEvent[] events);
}
