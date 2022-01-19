using BuildingBlocks.Domain.Model;
using Catalog.Categories;
using Catalog.Core.Contracts;
using Catalog.Products;
using Catalog.Suppliers;

namespace Catalog.Infrastructure.Data;

public class CatalogDbContext : AppDbContextBase, ICatalogDbContext
{
    public const string DefaultSchema = "catalog";

    public CatalogDbContext(DbContextOptions options) : base(options)
    {
    }

    public CatalogDbContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity<long>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = 1;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = 1;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}
