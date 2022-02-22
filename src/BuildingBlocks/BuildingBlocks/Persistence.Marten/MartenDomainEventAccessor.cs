using BuildingBlocks.Abstractions.Domain.Events;
using BuildingBlocks.Abstractions.Domain.Events.Internal;

namespace BuildingBlocks.Persistence.Marten;

public class MartenDomainEventAccessor : IDomainEventsAccessor
{
    private readonly IAggregatesDomainEventsStore _aggregatesDomainEventsStore;

    public MartenDomainEventAccessor(IAggregatesDomainEventsStore aggregatesDomainEventsStore)
    {
        _aggregatesDomainEventsStore = aggregatesDomainEventsStore;
    }

    public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents
    {
        get
        {
            return _aggregatesDomainEventsStore.GetAllUncommittedEvents();
        }
    }
}
