using BuildingBlocks.Domain.Events;

namespace Catalog.Products.Features.AddProductStock;

public record ProductStockAddedDomainEvent(int NewStock) : DomainEvent;
