using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.CQRS.Event;

namespace BuildingBlocks.Domain;

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    public Queue<DomainEvent> DomainEvents { get; private set; }

    public void RemoveDomainEvent(DomainEvent eventItem)
    {
        DomainEvents?.ToList().Remove(eventItem);
    }

    public virtual void When(object @event) { }

    public DomainEvent[] DequeueUncommittedEvents()
    {
        var dequeuedEvents = DomainEvents.ToArray();

        DomainEvents.Clear();

        return dequeuedEvents;
    }

    protected void Enqueue(DomainEvent @event)
    {
        DomainEvents.Enqueue(@event);
    }
}

public abstract class AggregateRoot : AggregateRoot<Guid>
{
}
