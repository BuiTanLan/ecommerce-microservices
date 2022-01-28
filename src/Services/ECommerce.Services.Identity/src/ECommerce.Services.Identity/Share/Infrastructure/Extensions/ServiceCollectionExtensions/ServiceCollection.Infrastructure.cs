using System.Reflection;
using BuildingBlocks.Caching;
using BuildingBlocks.CQRS;
using BuildingBlocks.Email;
using BuildingBlocks.Logging;
using BuildingBlocks.Monitoring;
using BuildingBlocks.Validation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Services.Identity.Share.Infrastructure.Extensions.ServiceCollectionExtensions;

public static class ServiceCollection
{
    public static WebApplicationBuilder AddInfrastructure(
        this WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        AddInfrastructure(builder.Services, configuration);

        return builder;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEmailService(configuration);

        services.AddCustomValidators(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddCqrs();

        services.AddMonitoring();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));

        services.AddCachingRequestPolicies(new List<Assembly> { Assembly.GetExecutingAssembly() });
        services.AddEasyCaching(options => { options.UseInMemory(configuration, "mem"); });

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMonitoring();

        return app;
    }
}
