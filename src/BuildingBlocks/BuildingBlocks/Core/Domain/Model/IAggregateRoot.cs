using BuildingBlocks.Core.Domain.Events.Internal;

namespace BuildingBlocks.Core.Domain.Model;

public interface IAggregateRoot : IAggregateRoot<Guid>, IEntity
{
}

/// <summary>
/// The aggregate root interface.
/// </summary>
/// <typeparam name="TId">The generic identifier.</typeparam>
public interface IAggregateRoot<out TId> : IEntity<TId>
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    TId Version { get; }

    /// <summary>
    /// Add the <paramref name="domainEvent"/> on the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    void AddDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Remove a domain event from the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    void RemoveDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events from the aggregate root.
    /// </summary>
    void ClearDomainEvents();
}
