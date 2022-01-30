using BuildingBlocks.Resiliency;
using ECommerce.Services.Customers.Customers.Clients;

namespace ECommerce.Services.Customers.Shared.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHttpClients(
        this IServiceCollection services,
        IConfiguration configuration,
        string pollySectionName = "PolicyConfig")
    {
        services.AddOptions<IdentityApiClientOptions>().Bind(configuration.GetSection(nameof(IdentityApiClientOptions)))
            .ValidateDataAnnotations();

        services.AddHttpClient<IIdentityApiClient, IdentityApiClient>()
            .AddCustomPolicyHandlers(configuration, pollySectionName);

        return services;
    }
}
