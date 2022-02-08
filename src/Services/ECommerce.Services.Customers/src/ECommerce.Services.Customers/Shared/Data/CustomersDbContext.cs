using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.RestockSubscriptions.Models;
using ECommerce.Services.Customers.Shared.Contracts;

namespace ECommerce.Services.Customers.Shared.Data;

public class CustomersDbContext : AppDbContextBase, ICustomersDbContext
{
    public const string DefaultSchema = "customer";

    public CustomersDbContext(DbContextOptions options) : base(options)
    {
    }

    public CustomersDbContext(DbContextOptions options, IDomainEventPublisher domainEventPublisher)
        : base(options, domainEventPublisher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Constants.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<RestockSubscription> RestockSubscriptions => Set<RestockSubscription>();
}
