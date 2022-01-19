using Catalog.Categories;
using Catalog.Products;
using Catalog.Suppliers;

namespace Catalog.Core.Contracts;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Supplier> Suppliers { get; }

    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
