using BuildingBlocks.Core.Domain.Events;
using ECommerce.Services.Customers;
using ECommerce.Services.Customers.Customers;
using ECommerce.Services.Customers.Customers.Data;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

namespace ECommerce.Services.Catalogs.Products;

internal static class CustomersConfigs
{
    public const string Tag = "Customers";
    public const string CustomersPrefixUri = $"{CustomersModuleConfiguration.CustomerModulePrefixUri}";

    internal static IServiceCollection AddCustomersServices(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, CustomersDataSeeder>();
        services.AddSingleton<IIntegrationEventMapper, CustomersEventMapper>();

        return services;
    }

    internal static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCompleteCustomerEndpoint();
        endpoints.MapCreateCustomerEndpoint();

        return endpoints;
    }
}
