using System.Threading.Channels;
using BuildingBlocks.Domain.Events.External;

namespace BuildingBlocks.Messaging.Transport.InMemory.Channels;

public class MessageChannel : IMessageChannel
{
    private readonly Channel<IIntegrationEvent> _messages = Channel.CreateUnbounded<IIntegrationEvent>();
    public ChannelReader<IIntegrationEvent> Reader => _messages.Reader;
    public ChannelWriter<IIntegrationEvent> Writer => _messages.Writer;
}
