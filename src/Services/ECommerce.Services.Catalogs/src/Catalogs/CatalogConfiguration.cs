using Catalogs.Brands;
using Catalogs.Categories;
using Catalogs.Products;
using Catalogs.Shared.Infrastructure.Extensions.ApplicationBuilderExtensions;
using Catalogs.Shared.Infrastructure.Extensions.ServiceCollectionExtensions;
using Catalogs.Suppliers;

namespace Catalogs;

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
        endpoints.MapGet("/", () => "Catalogs Service Apis").ExcludeFromDescription();
        endpoints.MapProductsEndpoints();

        return endpoints;
    }

    public static async Task ConfigureCatalog(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        app.UseInfrastructure();

        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
    }
}
