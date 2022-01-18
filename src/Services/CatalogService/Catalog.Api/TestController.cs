using BuildingBlocks.Domain.Events;
using BuildingBlocks.Messaging.Outbox;
using Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Api;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IBusPublisher _busPublisher;
    private readonly IOutboxService _outboxService;

    public TestController(IBusPublisher busPublisher, IOutboxService outboxService)
    {
        _busPublisher = busPublisher;
        _outboxService = outboxService;
    }

    [HttpPost("publish")]
    public async Task<ActionResult> Publish()
    {
        await _busPublisher.PublishAsync(new TestIntegrationEvent { Data = "Test" });

        return Ok();
    }

    [HttpPost("outbox")]
    public async Task<ActionResult> Outbox()
    {
        await _outboxService.SaveAsync(new TestIntegrationEvent { Data = "Test" });

        return Ok();
    }
}
