namespace BuildingBlocks.Core.Domain.Events.Internal;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken);
}
