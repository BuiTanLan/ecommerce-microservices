using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Catalogs.Suppliers.Features.SupplierCreated.External;

public record SupplierCreatedIntegrationEvent(long Id, string Name) : IntegrationEvent;
