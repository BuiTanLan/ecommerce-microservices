namespace BuildingBlocks.Domain.Events;

public interface IBusSubscriber
{
    Task StartAsync(CancellationToken cancellationToken = default);
}
