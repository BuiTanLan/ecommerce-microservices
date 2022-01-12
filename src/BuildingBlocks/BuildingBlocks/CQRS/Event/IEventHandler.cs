using MediatR;

namespace BuildingBlocks.CQRS.Event;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent
{
}
