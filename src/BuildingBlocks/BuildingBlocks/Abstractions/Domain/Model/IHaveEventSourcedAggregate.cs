using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Core.Objects.Versioning;

namespace BuildingBlocks.Abstractions.Domain.Model;

public interface IHaveEventSourcedAggregate : IHaveVersion
{
    void ApplyEvent(IDomainEvent @event, int version);

    /// <summary>
    /// Returns all uncommitted events and clears this events from the aggregate.
    /// </summary>
    /// <returns>Array of new uncommitted events.</returns>
    public IReadOnlyList<IDomainEvent> FlushUncommittedEvents();

    public IReadOnlyList<IDomainEvent> GetUncommittedEvents();
}
