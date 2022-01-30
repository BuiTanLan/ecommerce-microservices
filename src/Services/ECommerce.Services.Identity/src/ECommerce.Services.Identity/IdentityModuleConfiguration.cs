using ECommerce.Services.Identity.Identity;
using ECommerce.Services.Identity.Shared.Extensions.ApplicationBuilderExtensions;
using ECommerce.Services.Identity.Shared.Extensions.ServiceCollectionExtensions;
using ECommerce.Services.Identity.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Services.Identity;

public static class IdentityModuleConfiguration
{
    public const string IdentityModulePrefixUri = "api/v1/identity";

    public static WebApplicationBuilder AddIdentityModule(this WebApplicationBuilder builder)
    {
        AddIdentityModuleServices(builder.Services, builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddIdentityModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddIdentityServices(configuration);
        services.AddUsersServices(configuration);

        return services;
    }

    public static IEndpointRouteBuilder MapIdentityModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "ECommerce.Services.Identity Server Apis").ExcludeFromDescription();

        endpoints.MapIdentityEndpoints();
        endpoints.MapUsersEndpoints();

        return endpoints;
    }

    public static async Task ConfigureIdentityModule(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        app.UseInfrastructure();
        app.UseIdentityServer();

        await app.ApplyDatabaseMigrations(logger);
        await app.SeedData(logger, environment);
    }
}
