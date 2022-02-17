using ECommerce.Services.Catalogs.Brands;
using ECommerce.Services.Catalogs.Categories;
using ECommerce.Services.Catalogs.Products;
using ECommerce.Services.Catalogs.Shared.Extensions.ApplicationBuilderExtensions;
using ECommerce.Services.Catalogs.Shared.Extensions.ServiceCollectionExtensions;
using ECommerce.Services.Catalogs.Suppliers;

namespace ECommerce.Services.Catalogs;

public static class CatalogModuleConfiguration
{
    public const string CatalogModulePrefixUri = "api/v1/catalogs";

    public static WebApplicationBuilder AddCatalogModuleServices(this WebApplicationBuilder builder)
    {
        AddCatalogModuleServices(builder.Services, builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddCatalogModuleServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddStorage(configuration);

        services.AddBrandsServices();
        services.AddCategoriesServices();
        services.AddSuppliersServices();

        services.AddProductsServices();

        return services;
    }

    public static IEndpointRouteBuilder MapCatalogModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", (HttpContext context) =>
        {
            var requestId = context.Request.Headers.TryGetValue("X-Request-Id", out var requestIdHeader)
                ? requestIdHeader.FirstOrDefault()
                : string.Empty;

            return $"Catalogs Service Apis, RequestId: {requestId}";
        }).ExcludeFromDescription();

        endpoints.MapProductsEndpoints();

        return endpoints;
    }

    public static async Task ConfigureCatalogModule(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        app.UseInfrastructure();

        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
    }
}
