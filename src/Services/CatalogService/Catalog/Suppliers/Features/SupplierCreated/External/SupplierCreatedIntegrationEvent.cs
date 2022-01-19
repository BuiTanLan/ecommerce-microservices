using BuildingBlocks.Domain.Events.External;

namespace Catalog.Suppliers.Features.SupplierCreated.External;

public record SupplierCreatedIntegrationEvent(long Id, string Name) : IntegrationEvent;
