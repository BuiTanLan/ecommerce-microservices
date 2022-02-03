using System.Reflection;
using BuildingBlocks.Messaging.MassTransit.Options;
using BuildingBlocks.Web.Extensions;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.MassTransit
{
    // https://www.youtube.com/watch?v=bsUlQ93j2MY
    // https://masstransit-project.com/advanced/topology/message.html
    // https://masstransit-project.com/advanced/topology/rabbitmq.html
    // https://masstransit-project.com/advanced/topology/conventions.html
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.SetKebabCaseEndpointNameFormatter();

                // exclude namespace for the messages
                configure.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(false));

                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    var rabbitmqOptions =
                        configuration.GetOptions<MassTransitRabbitMQOptions>(nameof(MassTransitRabbitMQOptions));

                    configurator.Host(rabbitmqOptions.HostName);

                    configurator.ConfigureEndpoints(context);

                    configurator.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}
