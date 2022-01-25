using BuildingBlocks.Core.Domain.Events.Internal;

namespace BuildingBlocks.Core.Domain.Model;

public interface IHaveAggregate
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Add the <paramref name="domainEvent"/> on the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Remove a domain event from the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events from the aggregate root.
    /// </summary>
    void ClearDomainEvents();

    void IncrementVersion();
}
