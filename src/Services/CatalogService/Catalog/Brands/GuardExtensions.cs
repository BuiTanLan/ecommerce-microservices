using Ardalis.GuardClauses;
using Catalog.Brands.Exceptions.Application;

namespace Catalog.Brands;

public static class GuardExtensions
{
    public static void BrandNotFound(this IGuardClause guardClause, Brand? brand, long brandId)
    {
        if (brand == null)
            throw new BrandNotFoundException(brandId);
    }

    public static void BrandNotFound(this IGuardClause guardClause, bool exists, long brandId)
    {
        if (exists == false)
            throw new BrandNotFoundException(brandId);
    }
}
