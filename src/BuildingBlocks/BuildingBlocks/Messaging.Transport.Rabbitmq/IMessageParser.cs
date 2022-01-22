using BuildingBlocks.Core.Domain.Events.External;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Transport.Rabbitmq;

public interface IMessageParser
{
    IIntegrationEvent Resolve(IBasicProperties basicProperties, byte[] body);
}
