using Newtonsoft.Json;

namespace BuildingBlocks.Domain.Events;

public class DomainNotificationEvent : Event, IDomainNotificationEvent
{
    protected DomainNotificationEvent()
    {
        OccurredOn = DateTime.Now;
        Id = Guid.NewGuid();
    }

    [JsonConstructor]
    protected DomainNotificationEvent(Guid id, DateTime occursOn)
    {
        OccurredOn = occursOn;
        Id = id;
    }

    public new string EventType { get { return GetType().FullName; } }
}
