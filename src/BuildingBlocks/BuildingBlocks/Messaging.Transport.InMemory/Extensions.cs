using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.Transport;
using BuildingBlocks.Messaging.Transport.InMemory.Channels;
using BuildingBlocks.Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryTransport(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IBusPublisher, InMemoryPublisher>()
                .AddSingleton<IBusSubscriber, InMemorySubscriber>()
                .AddTransient<InMemoryProducerDiagnostics>()
                .AddTransient<InMemoryConsumerDiagnostics>();

            services.AddSingleton<IMessageChannel, MessageChannel>();
            return services;
        }
    }
}
