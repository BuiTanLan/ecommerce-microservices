namespace BuildingBlocks.Core.Domain.Events.External;

public abstract record IntegrationEvent : Event, IIntegrationEvent
{
    public string CorrelationId { get; protected set; } = default;
}
