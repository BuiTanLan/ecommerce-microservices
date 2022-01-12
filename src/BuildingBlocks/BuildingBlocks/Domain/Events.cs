using MediatR;

namespace BuildingBlocks.Domain;

public class EventWrapper : INotification
{
    public EventWrapper(IDomainEvent @event)
    {
        Event = @event;
    }

    public IDomainEvent Event { get; }
}
