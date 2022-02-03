using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Messaging.Outbox.EF;

public interface IOutboxDataContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
