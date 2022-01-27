using BuildingBlocks.Core.Domain.Events;
using Catalogs.Suppliers.Features.SupplierCreated.External;

namespace Catalogs.Suppliers.Features.SupplierCreated;

public class SupplierCreatedConsumer : IEventHandler<SupplierCreatedIntegrationEvent>
{
    public Task Handle(SupplierCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
