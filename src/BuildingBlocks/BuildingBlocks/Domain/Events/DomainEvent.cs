namespace BuildingBlocks.Domain.Events;

public abstract class DomainEvent : Event, IDomainEvent
{
    public string EventType { get { return GetType().FullName; } }
}
