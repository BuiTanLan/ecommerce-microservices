using Ardalis.GuardClauses;
using ECommerce.Services.Customers.RestockSubscriptions.Features.ProcessingRestockNotification;
using MicroBootstrap.Abstractions.Core.Domain.Events.External;
using MicroBootstrap.Abstractions.CQRS.Command;
using MicroBootstrap.Core.Domain.Events.External;

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

    // If this handler is called successfully, it will send a ACK to rabbitmq for removing message from the queue and if we have an exception it send an NACK to rabbitmq
    // and with NACK we can retry the message with re-queueing this message to the broker
    public async Task Handle(ProductStockReplenished notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        await _commandProcessor.SendAsync(
            new ProcessRestockNotification(notification.ProductId, notification.NewStock),
            cancellationToken);

        _logger.LogInformation("Sending restock notification command for product {ProductId}", notification.ProductId);
    }
}
