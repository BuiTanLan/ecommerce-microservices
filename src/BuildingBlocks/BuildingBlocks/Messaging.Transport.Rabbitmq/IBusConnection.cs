using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Transport.Rabbitmq;

public interface IBusConnection
{
    bool IsConnected { get; }
    IModel CreateChannel();
}
