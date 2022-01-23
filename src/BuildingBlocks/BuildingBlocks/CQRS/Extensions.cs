using BuildingBlocks.CQRS.Command;
using BuildingBlocks.CQRS.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CQRS;

public static class Extensions
{
    public static WebApplicationBuilder AddCqrs(this WebApplicationBuilder builder)
    {
        builder.Services.AddCqrs();

        return builder;
    }

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddTransient<ICommandProcessor, CommandProcessor>()
            .AddTransient<IQueryProcessor, QueryProcessor>();

        return services;
    }
}
