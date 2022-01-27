using Ardalis.GuardClauses;
using Catalogs.Products.Exceptions.Application;
using Catalogs.Products.Models;

namespace Catalogs.Products;

public static class GuardExtensions
{
    public static Product NullProduct(this IGuardClause guardClause, Product? product, long productId)
    {
        if (product == null)
            throw new ProductNotFoundException(productId);
        return product;
    }

    public static void ExistsProduct(this IGuardClause guardClause, bool exists, long productId)
    {
        if (exists == false)
            throw new ProductNotFoundException(productId);
    }
}
