using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Messaging.Outbox;
using Catalog.Brands;
using Catalog.Categories;
using Catalog.Products.Models;
using Catalog.Shared.Core.Contracts;
using Catalog.Suppliers;

namespace Catalog.Shared.Infrastructure.Data;

public class CatalogDbContext : AppDbContextBase, ICatalogDbContext
{
    public const string DefaultSchema = "catalog";

    public CatalogDbContext(DbContextOptions options) : base(options)
    {
    }

    public CatalogDbContext(DbContextOptions options, IMediator mediator, IOutboxService outboxService)
        : base(options, mediator, outboxService)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Constants.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductView> ProductsView => Set<ProductView>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Brand> Brands => Set<Brand>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity<long>>())
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

        foreach (var entry in ChangeTracker.Entries<IEntity<long>>())
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
