using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.AddingProductStock;

public record ProductStockAddedDomainEvent(int NewStock) : DomainEvent;
