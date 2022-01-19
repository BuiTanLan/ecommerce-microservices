using BuildingBlocks.Domain.Events.External;

namespace Catalog.Suppliers.Features.SupplierDeleted.External;

public record SupplierDeletedIntegrationEvent(long Id) : IntegrationEvent;
