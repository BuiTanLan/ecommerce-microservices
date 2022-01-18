using System.Threading.Channels;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Events.External;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryTransport(this IServiceCollection services)
        {
            services.AddSingleton<IBusPublisher, InMemoryPublisher>()
                .AddSingleton<IBusSubscriber, InMemorySubscriber>()
                .AddTransient<InMemoryProducerDiagnostics>()
                .AddTransient<InMemoryConsumerDiagnostics>();

            services.AddSingleton<IChannelFactory, ChannelFactory>();

            services.AddSingleton(_ => Channel.CreateUnbounded<IIntegrationEvent>())
                .AddSingleton(ctx =>
                {
                    var channel = ctx.GetService<Channel<IIntegrationEvent>>();
                    return channel?.Reader;
                }).AddSingleton(ctx =>
                {
                    var channel = ctx.GetService<Channel<IIntegrationEvent>>();
                    return channel?.Writer;
                });

            return services;
        }
    }
}
