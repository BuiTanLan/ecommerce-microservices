namespace BuildingBlocks.Domain.Events;

public interface IDomainEventContext
{
    IEnumerable<IDomainEvent> GetDomainEvents();
}
