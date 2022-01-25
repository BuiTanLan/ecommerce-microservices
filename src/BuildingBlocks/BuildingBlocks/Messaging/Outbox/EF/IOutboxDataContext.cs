using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Messaging.Outbox;

public interface IOutboxDataContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
