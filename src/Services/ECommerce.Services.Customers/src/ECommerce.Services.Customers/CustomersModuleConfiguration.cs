using BuildingBlocks.Web.Module;
using ECommerce.Services.Customers.Shared.Extensions.ApplicationBuilderExtensions;
using ECommerce.Services.Customers.Shared.Extensions.ServiceCollectionExtensions;

namespace ECommerce.Services.Customers;

public class CustomersModuleConfiguration : IRootModuleDefinition
{
    public const string CustomerModulePrefixUri = "api/v1/customers";

    public IServiceCollection AddModuleServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddCustomHttpClients(configuration);
        services.AddStorage(configuration);

        return services;
    }

    public async Task<WebApplication> ConfigureModule(WebApplication app)
    {
        app.UseInfrastructure();

        await app.ApplyDatabaseMigrations(app.Logger);
        await app.SeedData(app.Logger, app.Environment);

        return app;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "ECommerce.Services.Customers Service Apis").ExcludeFromDescription();

        return endpoints;
    }
}
