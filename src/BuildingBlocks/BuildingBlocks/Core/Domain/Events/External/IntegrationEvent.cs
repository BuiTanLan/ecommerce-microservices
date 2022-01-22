namespace BuildingBlocks.Core.Domain.Events.External;

public abstract record IntegrationEvent : Event, IIntegrationEvent
{
    public new Guid EventId { get; protected set; } = Guid.NewGuid();
    public new int EventVersion { get; protected set; } = 1;
    public new DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    public string CorrelationId { get; protected set; } = default;
}
