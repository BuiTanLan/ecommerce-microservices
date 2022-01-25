using Ardalis.GuardClauses;
using Catalog.Products.Core.Exceptions.Application;
using Catalog.Products.Core.Exceptions.Domain;
using Catalog.Products.Core.Models;
using Catalog.Products.Models;

namespace Catalog.Products.Infrastructure;

public static class GuardExtensions
{
    public static void ProductNotFound(this IGuardClause guardClause, Product? product, long productId)
    {
        if (product == null)
            throw new ProductNotFoundException(productId);
    }

    public static void ProductNotFound(this IGuardClause guardClause, bool exists, long productId)
    {
        if (exists == false)
            throw new ProductNotFoundException(productId);
    }
}
