using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions;

public static class ModuleDefinitionExtensions
{
    public static IServiceCollection AddModuleServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        var endpointDefinitions = new List<IModuleDefinition>();

        foreach (var assembly in assemblies)
        {
            endpointDefinitions.AddRange(
                assembly.ExportedTypes
                    .Where(s => typeof(IModuleDefinition).IsAssignableFrom(s) && !s.IsAbstract)
                    .Select(Activator.CreateInstance).Cast<IModuleDefinition>()
            );
        }

        endpointDefinitions.ForEach(s => s.AddModuleServices(services));
        services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IModuleDefinition>);

        return services;
    }

    public static IServiceCollection AddModuleServices<TModule>(this IServiceCollection services)
        where TModule : class
    {
        if (!typeof(TModule).IsAssignableTo(typeof(IModuleDefinition)))
        {
            throw new ArgumentException($"{nameof(TModule)} must be implemented {nameof(IModuleDefinition)}");
        }

        var endpoint = Activator.CreateInstance(typeof(TModule)) as IModuleDefinition;

        endpoint!.AddModuleServices(services);

        services.AddSingleton<TModule>();

        return services;
    }

    public static WebApplication ConfigureModule(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IReadOnlyCollection<IModuleDefinition>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.ConfigureModule(app);
        }

        return app;
    }

    public static WebApplication ConfigureModule<TModule>(this WebApplication app)
        where TModule : IModuleDefinition
    {
        var endpoint = app.Services.GetRequiredService<TModule>();
        endpoint.ConfigureModule(app);
        return app;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints, params Assembly[] assemblies)
    {
        var endpointDefinitions = new List<IModuleDefinition>();

        foreach (var assembly in assemblies)
        {
            endpointDefinitions.AddRange(
                assembly.ExportedTypes
                    .Where(s => typeof(IModuleDefinition).IsAssignableFrom(s) && !s.IsAbstract)
                    .Select(Activator.CreateInstance).Cast<IModuleDefinition>()
            );
        }

        endpointDefinitions.ForEach(s => s.MapEndpoints(endpoints));

        return endpoints;
    }

    public static IEndpointRouteBuilder MapEndpoints<TModule>(this IEndpointRouteBuilder endpoints)
        where TModule : class
    {
        if (!typeof(TModule).IsAssignableTo(typeof(IModuleDefinition)))
        {
            throw new ArgumentException($"{nameof(TModule)} must be implemented {nameof(IModuleDefinition)}");
        }

        var endpoint = Activator.CreateInstance(typeof(TModule)) as IModuleDefinition;

        endpoint!.MapEndpoints(endpoints);

        return endpoints;
    }
}
