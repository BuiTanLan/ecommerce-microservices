using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Messaging.BackgroundServices;

public class SubscribersBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SubscribersBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var subscribers = _serviceProvider.GetServices<IBusSubscriber>();
        await Task.WhenAll(subscribers.Select(s => s.StopAsync(cancellationToken)));

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscribers = _serviceProvider.GetServices<IBusSubscriber>();

        await Task.WhenAll(subscribers.Select(s => s.StartAsync(stoppingToken)));
    }
}
