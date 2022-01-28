using BuildingBlocks.Core.Domain.Events;
using ECommerce.Services.Catalogs.Suppliers.Features.SupplierDeleted.External;

namespace ECommerce.Services.Catalogs.Suppliers.Features.SupplierDeleted;

public class SupplierDeletedConsumer : IEventHandler<SupplierDeletedIntegrationEvent>
{
    public Task Handle(SupplierDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
