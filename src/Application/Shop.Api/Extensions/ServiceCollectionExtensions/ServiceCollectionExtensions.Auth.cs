namespace Microsoft.Extensions.DependencyInjection;

using AspNetCore.Authentication.JwtBearer;
using AspNetCore.Authorization;

public static partial class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        AddAuthentication(builder.Services);
        return builder;
    }
    public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        return builder;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = "https://localhost:7001";
            options.Audience = "api1";
            options.TokenValidationParameters.ValidateLifetime = false;
            options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
        });

        return services;
    }
}
