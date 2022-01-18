namespace BuildingBlocks.Domain.Events;

public interface IBusSubscriber
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
