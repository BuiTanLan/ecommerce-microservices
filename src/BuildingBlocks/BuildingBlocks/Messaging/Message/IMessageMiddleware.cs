namespace BuildingBlocks.Messaging.Message
{
    public interface IMessageMiddleware<TMessage> where TMessage : IMessage
    {
        Task RunAsync(TMessage message, IMessageContext messageContext, CancellationToken cancellationToken,
            HandleMessageDelegate<TMessage> next);
    }
}