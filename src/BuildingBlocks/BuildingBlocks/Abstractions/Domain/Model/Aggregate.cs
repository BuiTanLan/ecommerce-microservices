using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Exceptions;

namespace BuildingBlocks.Abstractions.Domain.Model;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    [NonSerialized] private readonly List<IDomainEvent> _uncommittedDomainEvents = new();

    private bool _versionIncremented;

    public int Version { get; protected set; }

    /// <summary>
    /// Gets the aggregate root domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _uncommittedDomainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        // if (!_uncommittedDomainEvents.Any() && !_versionIncremented)
        // {
        //     Version++;
        //     _versionIncremented = true;
        // }

        IDomainEvent eventWithAggregate = domainEvent.WithAggregate(Id, Version);

        ((IHaveEventSourcedAggregate)this).ApplyEvent(eventWithAggregate, Version + 1);

        _uncommittedDomainEvents.Add(eventWithAggregate);
    }

    public IReadOnlyList<IDomainEvent> FlushUncommittedEvents()
    {
        var events = _uncommittedDomainEvents.ToImmutableList();

        _uncommittedDomainEvents.Clear();

        return events;
    }

    public IReadOnlyList<IDomainEvent> GetUncommittedEvents()
    {
        return _uncommittedDomainEvents.ToImmutableList();
    }

    public void IncrementVersion()
    {
        if (_versionIncremented)
        {
            return;
        }

        Version++;
        _versionIncremented = true;
    }

    void IHaveEventSourcedAggregate.ApplyEvent(IDomainEvent @event, int version)
    {
        if (!_uncommittedDomainEvents.Any(x => Equals(x.EventId, @event.EventId)))
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version = version;
            _versionIncremented = true;
        }
    }

    public void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    public string IdAsString()
    {
        return $"{GetType().Name}-{Id.ToString()}";
    }

    public virtual void When(object @event)
    {
    }
}

public abstract class Aggregate<TIdentity, TId> : Aggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class Aggregate : Aggregate<AggregateId, long>, IAggregate
{
}
