using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.DebitingProductStock.Events.Domain;

public record ProductStockDebited(int NewStock, int DebitedQuantity) : DomainEvent;
