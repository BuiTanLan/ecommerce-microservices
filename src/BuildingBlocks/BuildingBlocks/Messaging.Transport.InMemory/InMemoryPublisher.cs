using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Transport.InMemory;

public class InMemoryPublisher : IBusPublisher
{
    private readonly ILogger<InMemoryPublisher> _logger;
    private readonly IChannelFactory _channelFactory;
    private readonly InMemoryProducerDiagnostics _producerDiagnostics;
    private readonly IServiceProvider _serviceProvider;

    public InMemoryPublisher(
        ILogger<InMemoryPublisher> logger,
        IChannelFactory channelFactory,
        InMemoryProducerDiagnostics producerDiagnostics,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _channelFactory = channelFactory;
        _producerDiagnostics = producerDiagnostics;
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        if (integrationEvent == null)
            throw new ArgumentNullException(nameof(integrationEvent));

        // if (message.CorrelationId == Guid.Empty)
        // {
        //     var context = _serviceProvider.GetRequiredService<ICorrelationContextAccessor>();
        //     message.CorrelationId = Guid.Parse(context.CorrelationId);
        // }
        if (_channelFactory.GetWriter<TEvent>() is not null)
        {
            _logger.LogInformation("publishing message '{message.Id}'...", integrationEvent.Id);

            // ProducerDiagnostics
            _producerDiagnostics.StartActivity(integrationEvent);
            await _channelFactory.GetWriter<TEvent>().WriteAsync(integrationEvent, cancellationToken);
            _producerDiagnostics.StopActivity(integrationEvent);
        }
        else
        {
            _logger.LogWarning(
                "no suitable publisher found for message '{message.Id}' with type '{typeof(T).FullName}' !",
                integrationEvent.Id,
                typeof(TEvent).FullName);
            _producerDiagnostics.NoSubscriberToPublish(integrationEvent);
        }
    }
}
