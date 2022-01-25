using BuildingBlocks.Core.Domain.Events.Internal;

namespace BuildingBlocks.Core.Domain.Model;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
{
    [NonSerialized] private readonly List<IDomainEvent> _domainEvents = new();
    private bool _versionIncremented;

    public int Version { get; protected set; }

    /// <summary>
    /// Gets the aggregate root domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (!_domainEvents.Any() && !_versionIncremented)
        {
            Version++;
            _versionIncremented = true;
        }

        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
        => _domainEvents?.Remove(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents?.Clear();

    public void IncrementVersion()
    {
        if (_versionIncremented)
        {
            return;
        }

        Version++;
        _versionIncremented = true;
    }
}

public abstract class AggregateRoot<TIdentity, TId> : AggregateRoot<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class AggregateRoot : AggregateRoot<AggregateId, long>, IAggregateRoot
{
}
