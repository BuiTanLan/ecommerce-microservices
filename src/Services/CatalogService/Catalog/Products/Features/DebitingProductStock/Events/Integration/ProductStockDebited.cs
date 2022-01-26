using BuildingBlocks.Core.Domain.Events.External;

namespace Catalog.Products.Features.DebitingProductStock.Events.Integration;

public record ProductStockDebited(int NewStock, int DebitedQuantity) : IntegrationEvent;
