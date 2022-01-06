using Identity.Api;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

//Ref:https://www.scottbrady91.com/identity-server/getting-started-with-identityserver-4
public static partial class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddCustomIdentityServer(this WebApplicationBuilder builder, string connString)
    {
        //Problem with .net core identity - will override our default authentication scheme `JwtBearerDefaults.AuthenticationScheme` to unwanted `Identity.Application` in `AddIdentity()` method .net identity
        //https://github.com/IdentityServer/IdentityServer4/issues/1525

        // builder.Services.AddDbContext<IdentityContext>(options =>
        //     options.UseSqlServer(connString,
        //         x => x.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(connString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(IdentityContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
        });

        builder.Services.AddScoped<IProfileService, IdentityProfileService>();

        builder.Services.AddIdentity<ApplicationIdentityUser, IdentityRole>(options => {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(options => {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddDeveloperSigningCredential()        //This is for dev only scenarios when you don’t have a certificate to use.
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<ApplicationIdentityUser>()
            .AddProfileService<IdentityProfileService>();

        return builder;
    }
}