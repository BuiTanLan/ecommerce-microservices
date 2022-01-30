using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Shared.Contracts;

namespace ECommerce.Services.Customers.Shared.Data;

public class CustomersDbContext : AppDbContextBase, ICustomersDbContext
{
    public const string DefaultSchema = "customer";

    public CustomersDbContext(DbContextOptions options) : base(options)
    {
    }

    public CustomersDbContext(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher) :
        base(options, domainEventDispatcher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Constants.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IHaveAudit>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = 1;
                    break;
                case EntityState.Added:
                    entry.Entity.CreatedBy = 1;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<IHaveEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = 1;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}
