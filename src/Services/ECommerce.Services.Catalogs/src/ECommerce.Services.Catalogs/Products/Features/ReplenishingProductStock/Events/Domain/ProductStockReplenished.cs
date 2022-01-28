using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ReplenishingProductStock.Events.Domain;

public record ProductStockReplenished(int NewStock, int ReplenishedQuantity) : DomainEvent;
