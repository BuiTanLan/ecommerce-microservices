using ECommerce.Services.Customers.Shared.Core;

namespace Customers.Api.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppOptions>().Bind(configuration.GetSection(nameof(AppOptions)))
            .ValidateDataAnnotations();

        return services;
    }
}
