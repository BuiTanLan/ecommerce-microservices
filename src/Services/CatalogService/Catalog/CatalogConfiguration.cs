using BuildingBlocks.Core;
using Catalog.Brands;
using Catalog.Categories;
using Catalog.Categories.Data;
using Catalog.Products;
using Catalog.Shared.Infrastructure.Extensions.ApplicationBuilderExtensions;
using Catalog.Shared.Infrastructure.Extensions.ServiceCollectionExtensions;
using Catalog.Suppliers;

namespace Catalog;

public static class CatalogConfiguration
{
    public const string CatalogModulePrefixUri = "api/v1/catalog";

    public static WebApplicationBuilder AddCatalogServices(this WebApplicationBuilder builder)
    {
        AddCatalogServices(builder.Services, builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddStorage(configuration);

        services.AddCategoriesServices()
            .AddProductsServices()
            .AddSuppliersServices()
            .AddBrandsServices();

        return services;
    }

    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "Catalog Service Apis").ExcludeFromDescription();
        endpoints.MapProductsEndpoints();

        return endpoints;
    }

    public static async Task ConfigureCatalog(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
    }
}
