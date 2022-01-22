using BuildingBlocks.Core.Domain.Events.External;

namespace BuildingBlocks.Messaging.Transport.Rabbitmq;

public interface IQueueReferenceFactory
{
    QueueReferences Create<TM>(TM message = default)
        where TM : IIntegrationEvent;
}
