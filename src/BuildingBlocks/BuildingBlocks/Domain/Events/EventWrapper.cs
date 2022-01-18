using MediatR;

namespace BuildingBlocks.Domain.Events;

public class EventWrapper : INotification
{
    public EventWrapper(IDomainEvent @event)
    {
        Event = @event;
    }

    public IDomainEvent Event { get; }
}
