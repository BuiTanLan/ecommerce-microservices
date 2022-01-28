using Ardalis.GuardClauses;
using ECommerce.Services.Catalogs.Suppliers.Exceptions.Application;

namespace ECommerce.Services.Catalogs.Suppliers;

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
