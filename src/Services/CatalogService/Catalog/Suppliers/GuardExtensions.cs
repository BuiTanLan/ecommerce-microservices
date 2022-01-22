using Ardalis.GuardClauses;
using Catalog.Products.Exceptions.Application;
using Catalog.Products.Models;
using Catalog.Suppliers.Exceptions.Application;

namespace Catalog.Suppliers;

public static class GuardExtensions
{
    public static void SupplierNotFound(this IGuardClause guardClause, Supplier product, long supplierId)
    {
        if (product == null)
            throw new SupplierNotFoundException(supplierId);
    }

    public static void SupplierNotFound(this IGuardClause guardClause, bool exists, long supplierId)
    {
        if (exists == false)
            throw new SupplierNotFoundException(supplierId);
    }
}
