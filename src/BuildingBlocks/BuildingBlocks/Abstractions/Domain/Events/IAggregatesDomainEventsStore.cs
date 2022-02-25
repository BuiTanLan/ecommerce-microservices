using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.Abstractions.Domain.Model;

namespace BuildingBlocks.Abstractions.Domain.Events;

public interface IAggregatesDomainEventsStore
{
    IReadOnlyList<IDomainEvent> AddEventsFrom<T>(T aggregate)
        where T : IHaveEventSourcedAggregate;

    IReadOnlyList<IDomainEvent> AddEventsFrom(object entity);

    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
}
