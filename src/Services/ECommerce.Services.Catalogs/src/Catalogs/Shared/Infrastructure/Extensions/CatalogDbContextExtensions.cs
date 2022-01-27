using Catalogs.Brands;
using Catalogs.Categories;
using Catalogs.Products.Models;
using Catalogs.Shared.Core.Contracts;
using Catalogs.Suppliers;

namespace Catalogs.Shared.Infrastructure.Extensions;

/// <summary>
/// Put some shared code between multiple feature here, for preventing duplicate some codes
/// Ref: https://www.youtube.com/watch?v=01lygxvbao4.
/// </summary>
public static class CatalogDbContextExtensions
{
    public static Task<bool> ProductExistsAsync(
        this ICatalogDbContext context,
        ProductId id,
        CancellationToken cancellationToken = default)
    {
        return context.Products.AnyAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<Product?> FindProductAsync(
        this ICatalogDbContext context,
        ProductId id,
        CancellationToken cancellationToken = default)
    {
        return context.Products.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<bool> SupplierExistsAsync(
        this ICatalogDbContext context,
        SupplierId id,
        CancellationToken cancellationToken = default)
    {
        return context.Suppliers.AnyAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<Supplier?> FindSupplierAsync(
        this ICatalogDbContext context,
        SupplierId id,
        CancellationToken cancellationToken = default)
    {
        return context.Suppliers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<bool> CategoryExistsAsync(
        this ICatalogDbContext context,
        CategoryId id,
        CancellationToken cancellationToken = default)
    {
        return context.Categories.AnyAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<Category?> FindCategoryAsync(
        this ICatalogDbContext context,
        CategoryId id,
        CancellationToken cancellationToken = default)
    {
        return context.Categories.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<bool> BrandExistsAsync(
        this ICatalogDbContext context,
        BrandId id,
        CancellationToken cancellationToken = default)
    {
        return context.Brands.AnyAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public static Task<Brand?> FindBrandAsync(
        this ICatalogDbContext context,
        BrandId id,
        CancellationToken cancellationToken = default)
    {
        return context.Brands.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }
}
