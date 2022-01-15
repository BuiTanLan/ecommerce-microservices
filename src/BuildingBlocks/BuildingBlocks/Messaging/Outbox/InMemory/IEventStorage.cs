using System.Collections.Concurrent;

namespace BuildingBlocks.Messaging.Outbox.InMemory;

public interface IEventStorage
{
    public IList<OutboxMessage> Events { get; }
}
