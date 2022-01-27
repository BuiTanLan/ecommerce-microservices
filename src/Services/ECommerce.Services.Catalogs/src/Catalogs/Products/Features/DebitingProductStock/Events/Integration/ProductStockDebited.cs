using BuildingBlocks.Core.Domain.Events.External;

namespace Catalogs.Products.Features.DebitingProductStock.Events.Integration;

public record ProductStockDebited(int NewStock, int DebitedQuantity) : IntegrationEvent;
