using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Domain.Events.Internal;

public interface IDomainEventContext
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    IReadOnlyList<(IHaveAggregate Aggregate, IReadOnlyList<IDomainEvent> DomainEvents)> GetAggregateDomainEvents();
}
