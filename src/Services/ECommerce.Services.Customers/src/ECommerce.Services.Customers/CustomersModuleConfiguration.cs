using ECommerce.Services.Catalogs.Products;
using ECommerce.Services.Customers.Shared.Extensions.ApplicationBuilderExtensions;
using ECommerce.Services.Customers.Shared.Extensions.ServiceCollectionExtensions;

namespace ECommerce.Services.Customers;

public static class CustomersModuleConfiguration
{
    public const string CustomerModulePrefixUri = "api/v1/customers";

    public static WebApplicationBuilder AddCustomersModuleServices(this WebApplicationBuilder builder)
    {
        AddCustomersModuleServices(builder.Services, builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddCustomersModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddStorage(configuration);

        services.AddCustomersServices(configuration);

        return services;
    }

    public static IEndpointRouteBuilder MapCustomersModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "ECommerce.Services.Customers Service Apis").ExcludeFromDescription();

        endpoints.MapCustomersEndpoints();

        return endpoints;
    }

    public static async Task ConfigureCustomersModule(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        app.UseInfrastructure();

        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
    }
}
