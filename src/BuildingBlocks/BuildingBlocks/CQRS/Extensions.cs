using System.Reflection;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.CQRS.Event;
using BuildingBlocks.CQRS.Event.External;
using BuildingBlocks.CQRS.Query;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.CQRS;

public static class Extensions
{
    public static WebApplicationBuilder AddCqrs(this WebApplicationBuilder builder, params Assembly[] scanAssemblies)
    {
        builder.Services.AddCqrs(scanAssemblies);

        return builder;
    }

    public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] scanAssemblies)
    {
        services.AddMediatR(scanAssemblies ?? new[] { Assembly.GetExecutingAssembly() })
            .AddScoped<ICommandProcessor, CommandProcessor>()
            .AddScoped<IQueryProcessor, QueryProcessor>();

        services.TryAddScoped<IEventProcessor, EventProcessor>();
        services.TryAddScoped<IExternalEventProducer, EmptyExternalEventProducer>();

        return services;
    }
}
