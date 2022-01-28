using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.DebitingProductStock.Events.Domain;

public record ProductStockDebited(int NewStock, int DebitedQuantity) : DomainEvent;
