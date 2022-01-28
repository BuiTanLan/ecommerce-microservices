using Ardalis.GuardClauses;
using ECommerce.Services.Catalogs.Brands.Exceptions.Application;

namespace ECommerce.Services.Catalogs.Brands;

public static class GuardExtensions
{
    public static void NullBrand(this IGuardClause guardClause, Brand? brand, long brandId)
    {
        if (brand == null)
            throw new BrandNotFoundException(brandId);
    }

    public static void ExistsBrand(this IGuardClause guardClause, bool exists, long brandId)
    {
        if (exists == false)
            throw new BrandNotFoundException(brandId);
    }
}
