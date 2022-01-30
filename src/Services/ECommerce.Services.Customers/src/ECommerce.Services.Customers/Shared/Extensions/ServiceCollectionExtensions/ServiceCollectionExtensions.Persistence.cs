using ECommerce.Services.Customers.Shared.Contracts;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Shared.Extensions.ServiceCollectionExtensions;

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
            services.AddDbContext<CustomersDbContext>(options =>
                options.UseInMemoryDatabase("ECommerce.Services.ECommerce.Services.Customers"));
        }
        else
        {
            services.AddPostgresDbContext<CustomersDbContext>(
                configuration.GetConnectionString("CustomersServiceConnection"));
        }

        services.AddScoped<ICustomersDbContext>(provider => provider.GetRequiredService<CustomersDbContext>());

        return services;
    }
}
