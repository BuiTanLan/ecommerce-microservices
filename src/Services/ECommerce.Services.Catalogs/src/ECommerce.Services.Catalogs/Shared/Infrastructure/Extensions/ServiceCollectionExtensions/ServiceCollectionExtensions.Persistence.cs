using ECommerce.Services.Catalogs.Shared.Core.Contracts;
using ECommerce.Services.Catalogs.Shared.Infrastructure.Data;

namespace ECommerce.Services.Catalogs.Shared.Infrastructure.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddStorage(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        AddStorage(builder.Services, configuration);

        return builder;
    }

    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseInMemoryDatabase("ECommerce.Services.ECommerce.Services.Catalogs"));
        }
        else
        {
            services.AddPostgresDbContext<CatalogDbContext>(
                configuration.GetConnectionString("CatalogServiceConnection"));
        }

        services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());

        return services;
    }
}
