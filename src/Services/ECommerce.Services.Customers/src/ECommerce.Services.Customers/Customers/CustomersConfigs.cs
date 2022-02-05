using BuildingBlocks.Core.Domain.Events;
using ECommerce.Services.Catalogs.Products.Features.GettingProducts;
using ECommerce.Services.Customers;
using ECommerce.Services.Customers.Customers;
using ECommerce.Services.Customers.Customers.Data;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomerWithIdentity;
using ECommerce.Services.Customers.Customers.Features.GettingCustomerById;
using ECommerce.Services.Customers.Customers.Features.LockingCustomer;
using ECommerce.Services.Customers.Customers.Features.UnlockingCustomer;
using ECommerce.Services.Customers.Customers.Features.VerifyingCustomer;
using ECommerce.Services.Customers.Shared.Extensions.ServiceCollectionExtensions;

namespace ECommerce.Services.Catalogs.Products;

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
