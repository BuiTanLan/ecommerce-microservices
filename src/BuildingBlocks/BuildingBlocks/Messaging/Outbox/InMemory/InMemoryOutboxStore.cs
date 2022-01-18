namespace BuildingBlocks.Messaging.Outbox.InMemory;

public class InMemoryOutboxStore : IInMemoryOutboxStore
{
    public IList<OutboxMessage> Events { get; } = new List<OutboxMessage>();
}
