namespace BuildingBlocks.Domain.Events;

public abstract record DomainNotificationEvent : Event, IDomainNotificationEvent
{
    public new Guid EventId { get; protected set; } = Guid.NewGuid();
    public new int EventVersion { get; protected set; } = 1;
    public new DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
}
