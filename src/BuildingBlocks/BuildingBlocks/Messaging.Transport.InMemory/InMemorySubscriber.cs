using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Messaging.Transport.InMemory.Channels;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Transport.InMemory;

public class InMemorySubscriber : IBusSubscriber
{
    private readonly InMemoryConsumerDiagnostics _consumerDiagnostics;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageChannel _messageChannel;
    private readonly ILogger<InMemorySubscriber> _logger;

    public InMemorySubscriber(
        InMemoryConsumerDiagnostics consumerDiagnostics,
        IServiceProvider serviceProvider,
        IMessageChannel messageChannel,
        ILogger<InMemorySubscriber> logger)
    {
        _consumerDiagnostics = consumerDiagnostics;
        _serviceProvider = serviceProvider;
        _messageChannel = messageChannel;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await ListenToMessagesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-2-advanced-channels/
        _messageChannel.Writer.Complete();
        return Task.CompletedTask;
    }

    private async Task ListenToMessagesAsync(CancellationToken cancellationToken)
    {
        if (_messageChannel.Reader == null)
            return;

        using var scope = _serviceProvider.CreateScope();
        var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

        // https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-1-getting-started/
        await foreach (var @event in _messageChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                // ConsumerDiagnostics
                _consumerDiagnostics.StartActivity(@event);

                // Publish to internal event bus
                await eventProcessor.PublishAsync(@event, cancellationToken);
                _consumerDiagnostics.StopActivity(@event);
            }
            catch (System.Exception e)
            {
                _logger.LogError(
                    e,
                    "an exception has occurred while processing '{FullName}' message '{Id}': {Message}",
                    @event.GetType().FullName,
                    @event.EventId,
                    e.Message);
                _consumerDiagnostics.StopActivity(@event);
            }
        }
    }
}
