using Ardalis.GuardClauses;
using Catalog.Products.Models;
using Catalog.Suppliers.Exceptions.Application;

namespace Catalog.Suppliers;

public static class GuardExtensions
{
    public static void NullSupplier(this IGuardClause guardClause, Supplier? product, long supplierId)
    {
        if (product == null)
            throw new SupplierNotFoundException(supplierId);
    }

    public static void ExistsSupplier(this IGuardClause guardClause, bool exists, long supplierId)
    {
        if (exists == false)
            throw new SupplierNotFoundException(supplierId);
    }
}
