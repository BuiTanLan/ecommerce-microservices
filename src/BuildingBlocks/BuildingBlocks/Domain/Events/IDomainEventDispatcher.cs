namespace BuildingBlocks.Domain.Events;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync();
}
