using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Messaging.Transport.Rabbitmq;

public interface IPublisherChannelFactory
{
    PublisherChannelContext Create(IIntegrationEvent message);
}
