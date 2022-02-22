using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.Abstractions.Domain.Events.External;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.RestockSubscriptions.Features.SendingRestockNotification;

namespace ECommerce.Services.Customers.Products.Features.ReplenishingProductStock.Events.External;

public record ProductStockReplenished(long ProductId, int NewStock, int ReplenishedQuantity) : IntegrationEvent;

internal class ProductStockReplenishedHandler : IIntegrationEventHandler<ProductStockReplenished>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly ILogger<ProductStockReplenishedHandler> _logger;

    public ProductStockReplenishedHandler(
        ICommandProcessor commandProcessor,
        ILogger<ProductStockReplenishedHandler> logger)
    {
        _commandProcessor = commandProcessor;
        _logger = logger;
    }

    public async Task Handle(ProductStockReplenished notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        // send internal command to process in background with outbox or message scheduler
        await _commandProcessor.SendAsync(
            new SendRestockNotification(notification.ProductId, notification.NewStock),
            cancellationToken);

        _logger.LogInformation("Sending restock notification command for product {ProductId}", notification.ProductId);
    }
}
