using System.Threading.Channels;
using BuildingBlocks.Domain.Events.External;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Transport.InMemory
{
    internal class ChannelFactory : IChannelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ChannelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ChannelWriter<TM> GetWriter<TM>()
            where TM : IIntegrationEvent =>
            _serviceProvider.GetService<ChannelWriter<TM>>();

        public ChannelReader<TM> GetReader<TM>()
            where TM : IIntegrationEvent =>
            _serviceProvider.GetService<ChannelReader<TM>>();
    }
}
