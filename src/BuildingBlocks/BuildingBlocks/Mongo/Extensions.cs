using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Mongo
{
    public static class Extensions
    {
        public static IServiceCollection AddMongoDbContext<TContext>(
            this IServiceCollection services, IConfiguration configuration, Action<MongoOptions>? configurator = null)
            where TContext : MongoDbContext
        {
            return services.AddMongoDbContext<TContext, TContext>(configuration, configurator);
        }

        public static IServiceCollection AddMongoDbContext<TContextService, TContextImplementation>(
            this IServiceCollection services, IConfiguration configuration, Action<MongoOptions>? configurator = null)
            where TContextService : IMongoDbContext
            where TContextImplementation : MongoDbContext, TContextService
        {
            var mongoOptions = configuration.GetSection(nameof(MongoOptions)).Get<MongoOptions>() ?? new MongoOptions();

            services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));
            if (configurator is { })
                services.Configure(nameof(MongoOptions), configurator);

            services.AddScoped(typeof(TContextService), typeof(TContextImplementation));
            services.AddScoped(typeof(TContextImplementation));

            services.AddScoped<IMongoDbContext>(sp => sp.GetRequiredService<TContextService>());

            return services;
        }
    }
}
