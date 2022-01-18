using BuildingBlocks.Domain.Events.External;

namespace Catalog;

public class TestIntegrationEvent : IntegrationEvent
{
    public string Data { get; set; }
}
