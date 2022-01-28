using BuildingBlocks.EFCore;
using ECommerce.Services.Identity.Features.GetClaims;
using ECommerce.Services.Identity.Features.Login;
using ECommerce.Services.Identity.Features.Logout;
using ECommerce.Services.Identity.Features.RefreshToken;
using ECommerce.Services.Identity.Features.RegisteringUser;
using ECommerce.Services.Identity.Features.RevokeRefreshToken;
using ECommerce.Services.Identity.Features.SendEmailVerificationCode;
using ECommerce.Services.Identity.Features.VerifyEmail;
using ECommerce.Services.Identity.Share.Infrastructure.Data;
using ECommerce.Services.Identity.Share.Infrastructure.Extensions.ApplicationBuilderExtensions;
using ECommerce.Services.Identity.Share.Infrastructure.Extensions.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Constants = ECommerce.Services.Identity.Share.Core.Constants;

namespace ECommerce.Services.Identity;

public static class IdentityConfiguration
{
    public const string IdentityModulePrefixUri = "api/v1/identity";

    public static WebApplicationBuilder AddIdentityModule(this WebApplicationBuilder builder)
    {
        AddIdentityServices(builder.Services, builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        services.AddCustomIdentity(configuration);
        services.AddCustomIdentityServer();

        return services;
    }

    public static IEndpointRouteBuilder MapIdentityModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "ECommerce.Services.Identity Server Apis").ExcludeFromDescription();

        endpoints.MapGet(
            $"{IdentityModulePrefixUri}/user-role",
            [Authorize(
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                Roles = Constants.Role.User)]
            () => new { Role = Constants.Role.User }).WithTags("ECommerce.Services.Identity");

        endpoints.MapGet(
            $"{IdentityModulePrefixUri}/admin-role",
            [Authorize(
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                Roles = Constants.Role.Admin)]
            () => new { Role = Constants.Role.Admin }).WithTags("ECommerce.Services.Identity");

        endpoints.MapRegisterNewUserEndpoint();
        endpoints.MapLoginUserEndpoint();
        endpoints.MapLogoutEndpoint();
        endpoints.MapSendEmailVerificationCodeEndpoint();
        endpoints.MapSendVerifyEmailEndpoint();
        endpoints.MapRefreshTokenEndpoint();
        endpoints.MapRevokeTokenEndpoint();
        endpoints.MapGetClaimsEndpoint();

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
