using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Domain.Events;

public interface IDomainEventContext
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    IReadOnlyList<(IHaveAggregate Aggregate, IReadOnlyList<IDomainEvent> DomainEvents)> GetAggregateDomainEvents();
}
