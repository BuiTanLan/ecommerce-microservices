using BuildingBlocks.Core.Domain.Events.External;

namespace Catalogs.Products.Features.ReplenishingProductStock.Events.Integration;

public record ProductStockReplenished(int NewStock, int ReplenishedQuantity) : IntegrationEvent;
