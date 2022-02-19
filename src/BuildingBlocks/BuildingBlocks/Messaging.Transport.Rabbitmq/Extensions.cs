using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.Transport;
using BuildingBlocks.Messaging.Transport.Rabbitmq.Consumers;
using BuildingBlocks.Messaging.Transport.Rabbitmq.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Transport.Rabbitmq;

public static class Extensions
{
    public static IServiceCollection AddRabbitMqTransport(
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
}
