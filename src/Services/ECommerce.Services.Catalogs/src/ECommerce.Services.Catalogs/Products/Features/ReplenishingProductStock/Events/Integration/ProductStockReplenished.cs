using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Catalogs.Products.Features.ReplenishingProductStock.Events.Integration;

public record ProductStockReplenished(int NewStock, int ReplenishedQuantity) : IntegrationEvent;
