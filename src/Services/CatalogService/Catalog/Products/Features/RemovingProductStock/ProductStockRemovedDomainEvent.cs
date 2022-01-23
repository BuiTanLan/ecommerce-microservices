using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Domain.Events;

namespace Catalog.Products.Features.RemovingProductStock;

public record ProductStockRemovedDomainEvent(int NewStock) : DomainEvent;
