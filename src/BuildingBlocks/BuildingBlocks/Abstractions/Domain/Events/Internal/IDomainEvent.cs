namespace BuildingBlocks.Abstractions.Domain.Events.Internal;

/// <summary>
/// The domain event interface.
/// </summary>
public interface IDomainEvent : IEvent
{
    /// <summary>
    /// The identifier of the aggregate which has generated the event.
    /// </summary>
    dynamic AggregateId { get; }

    /// <summary>
    /// The version of the aggregate when the event has been generated.
    /// </summary>
    long AggregateVersion { get; }

    public IDomainEvent WithAggregate(dynamic aggregateId, long aggregateVersion);
}
