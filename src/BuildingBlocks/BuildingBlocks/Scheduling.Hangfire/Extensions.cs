using BuildingBlocks.Messaging.Scheduling;
using BuildingBlocks.Scheduling.Hangfire.MessagesScheduler;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BuildingBlocks.Scheduling.Hangfire
{
    public static class Extensions
    {
        public static IServiceCollection AddHangfireScheduler(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = configuration.GetSection(nameof(HangfireOptions)).Get<HangfireOptions>();
            services.AddOptions<HangfireOptions>().Bind(configuration.GetSection(nameof(HangfireOptions)))
                .ValidateDataAnnotations();

            var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            services.AddHangfire(hangfireConfiguration =>
            {
                if (options is null || options.UseInMemoryStorage ||
                    string.IsNullOrWhiteSpace(options.ConnectionString))
                {
                    hangfireConfiguration.UseInMemoryStorage();
                }
                else
                {
                    hangfireConfiguration.UsePostgreSqlStorage(options.ConnectionString);
                }

                hangfireConfiguration.UseSerializerSettings(jsonSettings);
            });
            services.AddHangfireServer();

            services.AddScoped<IHangfireMessagesScheduler, HangfireMessagesScheduler>();
            services.AddScoped<IMessagesScheduler, HangfireMessagesScheduler>();

            return services;
        }

        public static IApplicationBuilder UseHangfireScheduler(this IApplicationBuilder app)
        {
            return app.UseHangfireDashboard("/mydashboard");
        }
    }
}
