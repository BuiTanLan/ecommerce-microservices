using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Model;

/// <summary>
/// The aggregate root base class.
/// </summary>
/// <typeparam name="TId">The generic identifier.</typeparam>
public abstract class AggregateRoot<TId> : IAggregateRoot<TId>
{
    [NonSerialized] private List<DomainEvent> _domainEvents;

    /// <summary>
    /// Gets the aggregate root domain events.
    /// </summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    public TId Id { get; protected set; }

    public TId Version { get; protected set; } = default;

    /// <inheritdoc />
    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents ??= new List<DomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc />
    public void RemoveDomainEvent(DomainEvent domainEvent)
        => _domainEvents?.Remove(domainEvent);

    /// <inheritdoc />
    public void ClearDomainEvents()
        => _domainEvents?.Clear();
}

public abstract class AggregateRoot : AggregateRoot<Guid>
{
}
