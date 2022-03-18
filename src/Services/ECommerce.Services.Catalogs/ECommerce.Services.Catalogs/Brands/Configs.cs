using ECommerce.Services.Catalogs.Brands.Data;
using MicroBootstrap.Abstractions.Persistence;

namespace ECommerce.Services.Catalogs.Brands;

internal static class Configs
{
    internal static IServiceCollection AddBrandsServices(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, BrandDataSeeder>();

        return services;
    }

    internal static IEndpointRouteBuilder MapBrandsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}
