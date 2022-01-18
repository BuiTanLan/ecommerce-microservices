using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Transport.InMemory;

public class InMemorySubscriber : IBusSubscriber
{
    private readonly InMemoryConsumerDiagnostics _consumerDiagnostics;
    private readonly IServiceProvider _serviceProvider;
    private readonly IChannelFactory _channelFactory;
    private readonly ILogger<InMemorySubscriber> _logger;

    public InMemorySubscriber(
        InMemoryConsumerDiagnostics consumerDiagnostics,
        IServiceProvider serviceProvider,
        IChannelFactory channelFactory,
        ILogger<InMemorySubscriber> logger)
    {
        _consumerDiagnostics = consumerDiagnostics;
        _serviceProvider = serviceProvider;
        _channelFactory = channelFactory;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_channelFactory.GetReader<IIntegrationEvent>() == null)
            return Task.CompletedTask;

        return Task.Factory.StartNew(
            async () => await ListenToMessagesAsync(cancellationToken),
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-2-advanced-channels/
        _channelFactory.GetWriter<IIntegrationEvent>().Complete();
        return Task.CompletedTask;
    }

    private async Task ListenToMessagesAsync(CancellationToken cancellationToken)
    {
        var reader = _channelFactory.GetReader<IIntegrationEvent>();

        if (reader == null)
            return;
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-1-getting-started/
        await foreach (var message in reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                // ConsumerDiagnostics
                _consumerDiagnostics.StartActivity(message);
                await mediator.Publish(message, cancellationToken);
                _consumerDiagnostics.StopActivity(message);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e,
                    $"an exception has occurred while processing '{message.GetType().FullName}' message '{message.Id}': {e.Message}");
                _consumerDiagnostics.StopActivity(message);
            }
        }
    }
}
