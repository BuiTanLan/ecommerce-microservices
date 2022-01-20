using Catalog.Brands.Data;

namespace Catalog.Brands;

internal static class Configs
{
    internal static IServiceCollection AddBrands(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, BrandDataSeeder>();

        return services;
    }
}
