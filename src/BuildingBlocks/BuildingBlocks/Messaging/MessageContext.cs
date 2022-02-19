using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Messaging
{
    public class MessageContext : IMessageContext
    {
        public static MessageContext Default => new();
    }
}
