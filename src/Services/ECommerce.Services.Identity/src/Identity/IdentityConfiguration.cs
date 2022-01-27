using BuildingBlocks.EFCore;
using Identity.Features.ConfirmEmail;
using Identity.Features.GetClaims;
using Identity.Features.Login;
using Identity.Features.Logout;
using Identity.Features.RefreshToken;
using Identity.Features.RegisterNewUser;
using Identity.Features.RevokeRefreshToken;
using Identity.Features.SendEmailVerificationCode;
using Identity.Share.Infrastructure.Data;
using Identity.Share.Infrastructure.Extensions.ApplicationBuilderExtensions;
using Identity.Share.Infrastructure.Extensions.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Constants = Identity.Share.Core.Constants;

namespace Identity;

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
        endpoints.MapGet("/", () => "Identity Server Apis").ExcludeFromDescription();

        endpoints.MapGet(
            $"{IdentityModulePrefixUri}/user-role",
            [Authorize(
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                Roles = Constants.Role.User)]
            () => new { Role = Constants.Role.User }).WithTags("Identity");

        endpoints.MapGet(
            $"{IdentityModulePrefixUri}/admin-role",
            [Authorize(
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                Roles = Constants.Role.Admin)]
            () => new { Role = Constants.Role.Admin }).WithTags("Identity");

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
