using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Domain.Events;
using Catalog.Suppliers.Features.SupplierCreated.External;

namespace Catalog.Suppliers.Features.SupplierCreated;

public class SupplierCreatedConsumer : IEventHandler<SupplierCreatedIntegrationEvent>
{
    public Task Handle(SupplierCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
