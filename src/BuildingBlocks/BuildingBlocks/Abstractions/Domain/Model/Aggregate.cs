using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Exceptions;

namespace BuildingBlocks.Abstractions.Domain.Model;

// https://www.eventstore.com/blog/what-is-event-sourcing
// https://zimarev.com/blog/event-sourcing/entities-as-streams/
// https://event-driven.io/en/how_to_get_the_current_entity_state_in_event_sourcing/?utm_source=event_sourcing_net
// https://github.com/VenomAV/EventSourcingCQRS/blob/master/EventSourcingCQRS.Domain/Core/AggregateBase.cs
// https://github.com/gautema/CQRSlite/blob/master/Framework/CQRSlite/Domain/AggregateRoot.cs
// https://github.com/oskardudycz/EventSourcing.NetCore/blob/main/Core/Aggregates/Aggregate.cs
// https://github.com/Eventuous/eventuous/blob/dev/src/Core/src/Eventuous/Aggregate.cs
public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    [NonSerialized] private readonly List<IDomainEvent> _uncommittedDomainEvents = new();

    public int Version { get; protected set; }

    /// <summary>
    /// Gets the aggregate root domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _uncommittedDomainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        // if (!_uncommittedDomainEvents.Any() && !_version)
        // {
        //     Version++;
        //     _version = true;
        // }

        IDomainEvent eventWithAggregate = domainEvent.WithAggregate(Id, Version);

        ((IHaveEventSourcedAggregate)this).ApplyEvent(eventWithAggregate, Version + 1);

        _uncommittedDomainEvents.Add(eventWithAggregate);
    }

    public void Apply(IDomainEvent @event)
    {
        When(@event);
        Version++;
        AddDomainEvent(@event);
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

    void IHaveEventSourcedAggregate.ApplyEvent(IDomainEvent @event, int version)
    {
        if (!_uncommittedDomainEvents.Any(x => Equals(x.EventId, @event.EventId)))
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version = version;
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

    public abstract void When(IDomainEvent @event);
}

public abstract class Aggregate<TIdentity, TId> : Aggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

public abstract class Aggregate : Aggregate<AggregateId, long>, IAggregate
{
}
