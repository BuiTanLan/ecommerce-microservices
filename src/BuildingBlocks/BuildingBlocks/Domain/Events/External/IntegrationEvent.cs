namespace BuildingBlocks.Domain.Events.External;

public abstract class IntegrationEvent : Event, IIntegrationEvent
{
    public string CorrelationId { get; protected set; }
}
