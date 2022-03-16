using MicroBootstrap.Core.Domain.Events.External;

namespace ECommerce.Services.Catalogs.Products.Features.ReplenishingProductStock.Events.Integration;

public record ProductStockReplenished(long ProductId, int NewStock, int ReplenishedQuantity) : IntegrationEvent;
