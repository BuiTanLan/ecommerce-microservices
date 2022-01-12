using System.Collections.Generic;

namespace BuildingBlocks.Domain;

public interface IDomainEventContext
{
    IEnumerable<DomainEvent> GetDomainEvents();
}
