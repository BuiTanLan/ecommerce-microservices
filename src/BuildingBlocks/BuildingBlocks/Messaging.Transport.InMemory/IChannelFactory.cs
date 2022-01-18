using System.Threading.Channels;
using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Messaging.Transport.InMemory;

public interface IChannelFactory
{
    ChannelReader<TMessage> GetReader<TMessage>()
        where TMessage : IIntegrationEvent;

    ChannelWriter<TMessage> GetWriter<TMessage>()
        where TMessage : IIntegrationEvent;
}
