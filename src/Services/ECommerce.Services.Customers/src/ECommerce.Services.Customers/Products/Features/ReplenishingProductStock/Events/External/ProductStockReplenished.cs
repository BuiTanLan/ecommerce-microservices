using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Messaging.Scheduling;
using ECommerce.Services.Customers.RestockSubscriptions.Features.SendingRestockNotification;

namespace ECommerce.Services.Customers.Products.Features.ReplenishingProductStock.Events.External;

public record ProductStockReplenished(long ProductId, int NewStock, int ReplenishedQuantity) : IntegrationEvent;

internal class ProductStockReplenishedHandler : IIntegrationEventHandler<ProductStockReplenished>
{
    private readonly IMessagesScheduler _messagesScheduler;
    private readonly IOutboxService _outboxService;
    private readonly ILogger<ProductStockReplenishedHandler> _logger;

    public ProductStockReplenishedHandler(
        IMessagesScheduler messagesScheduler,
        IOutboxService outboxService,
        ILogger<ProductStockReplenishedHandler> logger)
    {
        _messagesScheduler = messagesScheduler;
        _outboxService = outboxService;
        _logger = logger;
    }

    public async Task Handle(ProductStockReplenished notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        // await _messagesScheduler.Enqueue(new SendRestockNotification(notification.ProductId, notification.NewStock));
        // Or
        await _outboxService.SaveAsync(
            new SendRestockNotification(notification.ProductId, notification.NewStock), cancellationToken);

        _logger.LogInformation("Sending restock notification command for product {ProductId}", notification.ProductId);
    }
}
