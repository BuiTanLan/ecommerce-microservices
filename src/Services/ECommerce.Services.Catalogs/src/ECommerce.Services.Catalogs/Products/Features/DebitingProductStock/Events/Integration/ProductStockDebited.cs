using BuildingBlocks.Abstractions.Domain.Events.External;

namespace ECommerce.Services.Catalogs.Products.Features.DebitingProductStock.Events.Integration;

public record ProductStockDebited(long ProductId, int NewStock, int DebitedQuantity) : IntegrationEvent;
