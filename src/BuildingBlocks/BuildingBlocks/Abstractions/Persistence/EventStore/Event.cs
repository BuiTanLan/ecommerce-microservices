using BuildingBlocks.Abstractions.Domain.Events.Internal;

namespace BuildingBlocks.Abstractions.Persistence.EventStore;

public class Event
{
    public Event(IDomainEvent domainEvent, long eventNumber)
    {
        DomainEvent = domainEvent;
        EventNumber = eventNumber;
    }

    public long EventNumber { get; }

    public IDomainEvent DomainEvent { get; }
}
