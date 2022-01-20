using Catalog.Products.Data;

namespace Catalog.Products;

internal static class Configs
{
    internal static IServiceCollection AddProducts(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, ProductDataSeeder>();

        return services;
    }
}
