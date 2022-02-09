using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Persistence;
using ECommerce.Services.Customers.Customers.Data;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer;
using ECommerce.Services.Customers.Customers.Features.GettingCustomerById;
using ECommerce.Services.Customers.Customers.Features.GettingCustomers;
using ECommerce.Services.Customers.Customers.Features.LockingCustomer;
using ECommerce.Services.Customers.Customers.Features.UnlockingCustomer;
using ECommerce.Services.Customers.Customers.Features.VerifyingCustomer;

namespace ECommerce.Services.Customers.Customers;

internal static class CustomersConfigs
{
    public const string Tag = "Customers";
    public const string CustomersPrefixUri = $"{CustomersModuleConfiguration.CustomerModulePrefixUri}";

    internal static IServiceCollection AddCustomersServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDataSeeder, CustomersDataSeeder>();
        services.AddSingleton<IIntegrationEventMapper, CustomersEventMapper>();

        return services;
    }

    internal static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCompleteCustomerEndpoint();
        endpoints.MapCreateCustomerEndpoint();
        endpoints.MapLockCustomerEndpoint();
        endpoints.MapUnlockCustomerEndpoint();
        endpoints.MapVerifyCustomerEndpoint();
        endpoints.MapGetCustomersEndpoint();
        endpoints.MapGetCustomerByIdEndpoint();

        return endpoints;
    }
}
