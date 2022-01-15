using MediatR;

namespace BuildingBlocks.Domain.Events;

/// <summary>
/// EventWrapper or DomainEvent Notification is an that will raise when specific domain event occurs
/// </summary>
public class EventWrapper : INotification
{
    public EventWrapper(IDomainEvent @event)
    {
        Event = @event;
    }

    public IDomainEvent Event { get; }
}
