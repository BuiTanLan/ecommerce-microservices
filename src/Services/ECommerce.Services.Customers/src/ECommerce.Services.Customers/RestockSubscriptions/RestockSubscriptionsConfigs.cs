using BuildingBlocks.Core.Domain.Events;
using ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription;
using ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionById;
using ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptions;
using ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionsByEmails;

namespace ECommerce.Services.Customers.RestockSubscriptions;

public static class RestockSubscriptionsConfigs
{
    public const string Tag = "RestockSubscriptions";

    public const string RestockSubscriptionsUrl =
        $"{CustomersModuleConfiguration.CustomerModulePrefixUri}/restock-subscriptions";

    internal static IServiceCollection RestockSubscriptionsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IIntegrationEventMapper, RestockSubscriptionsEventMapper>();

        return services;
    }

    internal static IEndpointRouteBuilder MapRestockSubscriptionsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCreateRestockSubscriptionEndpoint();
        endpoints.MapGetRestockSubscriptionByIdEndpoint();
        endpoints.MapGetRestockSubscriptionsEndpoint();
        endpoints.MapGetRestockSubscriptionsByEmailsEndpoints();

        return endpoints;
    }
}
