using BuildingBlocks.Core.Domain.Events.Internal;

namespace BuildingBlocks.Core.Domain.Events;

public interface IDomainEventContext
{
    IEnumerable<IDomainEvent> GetDomainEvents();
}
