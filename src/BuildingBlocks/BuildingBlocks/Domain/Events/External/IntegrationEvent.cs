using Newtonsoft.Json;

namespace BuildingBlocks.Domain.Events.External;

public abstract class IntegrationEvent : Event, IIntegrationEvent
{
    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.Now;
    }

    [JsonConstructor]
    protected IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        OccurredOn = createDate;
    }

    public string CorrelationId { get; protected set; }
}
