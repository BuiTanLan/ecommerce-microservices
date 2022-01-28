using ECommerce.Services.Customers.Shared.Infrastructure.Extensions.ApplicationBuilderExtensions;
using ECommerce.Services.Customers.Shared.Infrastructure.Extensions.ServiceCollectionExtensions;

namespace ECommerce.Services.Customers;

public static class CustomersConfiguration
{
    public const string CatalogModulePrefixUri = "api/v1/catalog";

    public static WebApplicationBuilder AddCustomersServices(this WebApplicationBuilder builder)
    {
        AddCustomersServices(builder.Services, builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddCustomersServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddStorage(configuration);


        return services;
    }

    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "ECommerce.Services.Customers Service Apis").ExcludeFromDescription();

        return endpoints;
    }

    public static async Task ConfigureCustomers(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        app.UseInfrastructure();

        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
    }
}
