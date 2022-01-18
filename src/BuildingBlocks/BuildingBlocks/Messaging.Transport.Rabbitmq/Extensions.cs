using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.Messaging.Transport.Rabbitmq.Consumers;
using BuildingBlocks.Messaging.Transport.Rabbitmq.Producers;
using BuildingBlocks.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Transport.Rabbitmq;

public static class Extensions
{
    public static IServiceCollection AddRabbitMQTransport(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<RabbitConfiguration> configurator = null)
    {
        services.AddSingleton<IQueueReferenceFactory, QueueReferenceFactory>();
        services.AddSingleton<IMessageParser, MessageParser>();
        services.AddSingleton<IBusPublisher, RabbitMqProducer>();
        services.AddSingleton<IBusSubscriber, RabbitMqConsumer>();
        services.AddSingleton<IPublisherChannelContextPool, PublisherChannelContextPool>();
        services.AddSingleton<IPublisherChannelFactory, PublisherChannelFactory>();

        // var messageTypes = ReflectionHelpers.GetAllTypesImplementingInterface<IIntegrationEvent>();
        // foreach (var messageType in messageTypes)
        //     AddBusSubscriber(services, typeof(RabitMQConsumer<>).MakeGenericType(messageType));
        services.Configure<RabbitConfiguration>(configuration.GetSection(nameof(RabbitConfiguration)));
        if (configurator is { })
            services.Configure(nameof(RabbitConfiguration), configurator);

        var config = configuration.GetSection(nameof(RabbitConfiguration)).Get<RabbitConfiguration>();

        services.AddSingleton<IConnectionFactory>(ctx =>
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = config.HostName,
                UserName = config.UserName,
                VirtualHost = string.IsNullOrWhiteSpace(config.VirtualHost) ? "/" : config.VirtualHost,
                Password = config.Password,
                Port = config.Port,
                DispatchConsumersAsync = true
            };
            return connectionFactory;
        });

        services.AddSingleton<IBusConnection, RabbitPersistentConnection>();
        services.AddSingleton(config);

        return services;
    }

    private static IServiceCollection AddBusSubscriber(IServiceCollection services, Type subscriberType)
    {
        if (!services.Any(s => s.ImplementationType == subscriberType))
            services.AddSingleton(typeof(IBusSubscriber), subscriberType);
        return services;
    }
}
