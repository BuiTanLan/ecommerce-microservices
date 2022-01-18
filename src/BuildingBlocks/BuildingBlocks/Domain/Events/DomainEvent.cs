using Newtonsoft.Json;

namespace BuildingBlocks.Domain.Events;

public abstract class DomainEvent : Event, IDomainEvent
{
    protected DomainEvent()
    {
        OccurredOn = DateTime.Now;
        Id = Guid.NewGuid();
    }

    [JsonConstructor]
    protected DomainEvent(Guid id, DateTime occursOn)
    {
        OccurredOn = occursOn;
        Id = id;
    }

    public new string EventType { get { return GetType().FullName; } }
}
