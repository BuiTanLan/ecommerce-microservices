namespace BuildingBlocks.Abstractions.Messaging.Transport;

public interface IBusSubscriber
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
