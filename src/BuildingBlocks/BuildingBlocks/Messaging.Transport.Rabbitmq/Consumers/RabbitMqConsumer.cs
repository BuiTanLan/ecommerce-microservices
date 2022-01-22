using System.Reflection;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Utils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace BuildingBlocks.Messaging.Transport.Rabbitmq.Consumers;

public class RabbitMqConsumer : IBusSubscriber
{
    private readonly IBusConnection _connection;
    private readonly IMessageParser _messageParser;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitConfiguration _rabbitCfg;
    private readonly IServiceProvider _serviceProvider;
    private IModel _channel;

    public RabbitMqConsumer(
        IBusConnection connection,
        IMessageParser messageParser,
        ILogger<RabbitMqConsumer> logger,
        RabbitConfiguration rabbitCfg,
        IServiceProvider serviceProvider)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageParser = messageParser ?? throw new ArgumentNullException(nameof(messageParser));
        _rabbitCfg = rabbitCfg ?? throw new ArgumentNullException(nameof(rabbitCfg));
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        var messageTypes = AppDomain.CurrentDomain.GetAssemblies().GetHandledIntegrationEventTypes();
        var fac = _serviceProvider.GetRequiredService<IQueueReferenceFactory>();

        foreach (var messageType in messageTypes)
        {
            // https://www.davidguida.net/dynamic-method-invocation-with-net-core/
            // https://www.thereformedprogrammer.net/using-net-generics-with-a-type-derived-at-runtime/
            MethodInfo methodInfo = typeof(IQueueReferenceFactory).GetMethod("Create");
            MethodInfo generic = methodInfo.MakeGenericMethod(messageType);

            // Or --> fac.Create((dynamic)Activator.CreateInstance(messageType));
            var queueReferences =
                generic.Invoke(fac, new object[] { null }) as QueueReferences;

            InitChannel(queueReferences);
            InitSubscription(queueReferences);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        StopChannel();
        return Task.CompletedTask;
    }

    private void InitSubscription(QueueReferences queueReferences)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += OnMessageReceivedAsync;

        _logger.LogInformation($"initializing subscription on queue '{queueReferences.QueueName}' ...");
        _channel.BasicConsume(queue: queueReferences.QueueName, autoAck: false, consumer: consumer);
    }

    private void InitChannel(QueueReferences queueReferences)
    {
        StopChannel();

        _channel = _connection.CreateChannel();

        _logger.LogInformation(
            $"initializing dead-letter queue '{queueReferences.DeadLetterQueue}' on exchange '{queueReferences.DeadLetterExchangeName}'...");

        _channel.ExchangeDeclare(exchange: queueReferences.DeadLetterExchangeName, type: ExchangeType.Topic);
        _channel.QueueDeclare(queue: queueReferences.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _channel.QueueBind(queueReferences.DeadLetterQueue,
            queueReferences.DeadLetterExchangeName,
            routingKey: queueReferences.DeadLetterQueue,
            arguments: null);

        _logger.LogInformation(
            $"initializing retry queue '{queueReferences.RetryQueueName}' on exchange '{queueReferences.RetryExchangeName}'...");

        _channel.ExchangeDeclare(exchange: queueReferences.RetryExchangeName, type: ExchangeType.Topic);
        _channel.QueueDeclare(queue: queueReferences.RetryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>()
            {
                { Headers.XMessageTTL, (int)_rabbitCfg.RetryDelay.TotalMilliseconds },
                { Headers.XDeadLetterExchange, queueReferences.ExchangeName },
                { Headers.XDeadLetterRoutingKey, queueReferences.QueueName }
            });
        _channel.QueueBind(queue: queueReferences.RetryQueueName,
            exchange: queueReferences.RetryExchangeName,
            routingKey: queueReferences.RoutingKey,
            arguments: null);

        _logger.LogInformation(
            $"initializing queue '{queueReferences.QueueName}' on exchange '{queueReferences.ExchangeName}'...");

        _channel.ExchangeDeclare(exchange: queueReferences.ExchangeName, type: ExchangeType.Topic);
        _channel.QueueDeclare(queue: queueReferences.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>()
            {
                { Headers.XDeadLetterExchange, queueReferences.DeadLetterExchangeName },
                { Headers.XDeadLetterRoutingKey, queueReferences.DeadLetterQueue }
            });
        _channel.QueueBind(queue: queueReferences.QueueName,
            exchange: queueReferences.ExchangeName,
            routingKey: queueReferences.RoutingKey,
            arguments: null);

        _channel.CallbackException += OnChannelException;
    }

    private void OnChannelException(object _, CallbackExceptionEventArgs ea)
    {
        _logger.LogError(ea.Exception, "the RabbitMQ Channel has encountered an error: {ExceptionMessage}",
            ea.Exception.Message);

        // TODO --> Provide argument
        // InitChannel();
        // InitSubscription();
    }


    private void StopChannel()
    {
        if (_channel is null)
            return;

        _channel.CallbackException -= OnChannelException;

        if (_channel.IsOpen)
            _channel.Close();

        _channel.Dispose();
        _channel = null;
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var consumer = sender as IBasicConsumer;
        var channel = consumer?.Model ?? _channel;

        IIntegrationEvent message;
        try
        {
            message = _messageParser.Resolve(eventArgs.BasicProperties, eventArgs.Body.ToArray());
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "an exception has occured while decoding queue message from Exchange '{ExchangeName}', message cannot be parsed. Error: {ExceptionMessage}",
                eventArgs.Exchange,
                ex.Message);
            channel.BasicReject(eventArgs.DeliveryTag, requeue: false);

            return;
        }

        _logger.LogInformation(
            "received message '{MessageId}' from Exchange '{ExchangeName}''. Processing...",
            eventArgs.BasicProperties.MessageId,
            eventArgs.Exchange);
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

            // Publish to internal event bus
            await eventProcessor.PublishAsync(message);

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (System.Exception ex)
        {
            HandleConsumerException(ex, eventArgs, channel, message, false);
        }
    }

    private void HandleConsumerException(
        System.Exception ex,
        BasicDeliverEventArgs deliveryProps,
        IModel channel,
        IIntegrationEvent message,
        bool requeue)
    {
        var errorMsg =
            "an error has occurred while processing Message '{MessageId}' from Exchange '{ExchangeName}' : {ExceptionMessage} . "
            + (requeue ? "Re-enqueuing...." : "Rejecting...");

        _logger.LogWarning(ex, errorMsg, message.EventId, deliveryProps.Exchange, ex.Message);

        if (!requeue)
            channel.BasicReject(deliveryProps.DeliveryTag, requeue: false);
        else
        {
            channel.BasicAck(deliveryProps.DeliveryTag, false);
            channel.BasicPublish(
                exchange: deliveryProps.Exchange + ".retry",
                routingKey: deliveryProps.RoutingKey,
                basicProperties: deliveryProps.BasicProperties,
                body: deliveryProps.Body);
        }
    }

    public void Dispose()
    {
        StopChannel();
    }
}
