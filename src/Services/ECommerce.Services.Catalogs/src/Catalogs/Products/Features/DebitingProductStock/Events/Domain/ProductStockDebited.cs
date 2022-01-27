using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Features.DebitingProductStock.Events.Domain;

public record ProductStockDebited(int NewStock, int DebitedQuantity) : DomainEvent;
