using BuildingBlocks.Domain.Events;

namespace Catalog.Products.Features.RemoveProductStock;

public record ProductStockRemovedDomainEvent(int NewStock) : DomainEvent;
