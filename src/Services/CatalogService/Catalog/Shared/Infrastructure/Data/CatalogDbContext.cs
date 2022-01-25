using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;
using Catalog.Brands;
using Catalog.Categories;
using Catalog.Products.Core.Models;
using Catalog.Products.Models;
using Catalog.Shared.Core.Contracts;
using Catalog.Suppliers;

namespace Catalog.Shared.Infrastructure.Data;

public class CatalogDbContext : AppDbContextBase, ICatalogDbContext
{
    public const string DefaultSchema = "catalog";

    public CatalogDbContext(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher) : base(options, domainEventDispatcher)
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
