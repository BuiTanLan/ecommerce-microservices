namespace BuildingBlocks.Messaging.Outbox.InMemory;

public class EventStorage : IEventStorage
{
    public IList<OutboxMessage> Events { get; } = new List<OutboxMessage>();
}
