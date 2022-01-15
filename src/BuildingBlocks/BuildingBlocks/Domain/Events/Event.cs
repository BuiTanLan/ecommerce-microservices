namespace BuildingBlocks.Domain.Events;

/// <inheritdoc cref="IEvent"/>
public abstract class Event : IEvent
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public int EventVersion { get; protected set; } = 1;

    public DateTime OccurredOn { get; protected set; } = DateTime.Now;

    public string EventType { get { return GetType().FullName; } }
}
