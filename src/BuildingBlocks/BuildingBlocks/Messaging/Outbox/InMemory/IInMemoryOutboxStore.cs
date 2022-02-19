using BuildingBlocks.Abstractions.Messaging.Outbox;

namespace BuildingBlocks.Messaging.Outbox.InMemory;

public interface IInMemoryOutboxStore
{
    public IList<OutboxMessage> Events { get; }
}
