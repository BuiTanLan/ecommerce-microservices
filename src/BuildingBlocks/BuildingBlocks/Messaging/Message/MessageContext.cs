namespace BuildingBlocks.Messaging.Message
{
    public class MessageContext : IMessageContext
    {
        public static MessageContext Default => new();
    }
}
