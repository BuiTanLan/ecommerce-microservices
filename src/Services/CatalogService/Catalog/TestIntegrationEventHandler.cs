using BuildingBlocks.Domain.Events;

namespace Catalog;

public class TestIntegrationEventHandler : IEventHandler<TestIntegrationEvent>
{
    public Task Handle(TestIntegrationEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
