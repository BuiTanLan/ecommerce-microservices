using BuildingBlocks.Domain.Events;
using Catalog.Suppliers.Features.SupplierDeleted.External;

namespace Catalog.Suppliers.Features.SupplierDeleted;

public class SupplierDeletedConsumer : IEventHandler<SupplierDeletedIntegrationEvent>
{
    public Task Handle(SupplierDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
