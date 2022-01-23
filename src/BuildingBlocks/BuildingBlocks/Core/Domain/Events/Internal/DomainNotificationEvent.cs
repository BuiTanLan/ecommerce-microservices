namespace BuildingBlocks.Core.Domain.Events.Internal;

public abstract record DomainNotificationEvent : Event, IDomainNotificationEvent
{
    public new Guid EventId { get; protected set; } = Guid.NewGuid();
    public new int EventVersion { get; protected set; } = 1;
    public new DateTime OccurredOn { get; protected set; } = DateTime.Now;
    public string CorrelationId { get; protected set; } = default;
}
