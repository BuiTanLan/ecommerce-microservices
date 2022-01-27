using BuildingBlocks.Core.Domain.Events.External;

namespace Catalogs.Suppliers.Features.SupplierCreated.External;

public record SupplierCreatedIntegrationEvent(long Id, string Name) : IntegrationEvent;
