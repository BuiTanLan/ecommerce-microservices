namespace BuildingBlocks.Abstractions.Domain.Events.Internal;

public abstract record DomainEvent : Event, IDomainEvent
{
    public dynamic AggregateId { get; protected set; } = null!;
    public long AggregateVersion { get; protected set; }

    public virtual IDomainEvent WithAggregate(dynamic aggregateId, long aggregateVersion)
    {
        AggregateId = aggregateId;
        AggregateVersion = aggregateVersion;

        return this;
    }
}
