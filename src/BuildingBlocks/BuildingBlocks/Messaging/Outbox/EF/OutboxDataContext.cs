using BuildingBlocks.Abstractions.Messaging.Outbox;
using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Messaging.Outbox.EF;

public class OutboxDataContext : AppDbContextBase
{
    /// <summary>
    /// The default database schema.
    /// </summary>
    public const string DefaultSchema = "messaging";

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public OutboxDataContext(DbContextOptions<OutboxDataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
    }
}
