using BuildingBlocks.Domain.Events.External;

namespace Catalog.Suppliers.Features.SupplierUpdated.External;

public record SupplierUpdatedIntegrationEvent(long Id, string Name) : IntegrationEvent;
