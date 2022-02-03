namespace BuildingBlocks.Messaging;

public interface IBusSubscriber
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
