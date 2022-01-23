using BuildingBlocks.Caching;
using BuildingBlocks.Logging;
using BuildingBlocks.Validation;

namespace Catalog.Shared.Infrastructure.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMediatR(
        this IServiceCollection services,
        Assembly[]? assemblies = null,
        Action<IServiceCollection> doMoreActions = null)
    {
        services.AddMediatR(assemblies ?? new[] { Assembly.GetExecutingAssembly() })
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));

        return services;
    }
}
