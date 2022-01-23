using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.ChangingProductSupplier;

internal record ChangeProductSupplierCommand : ICommand<ChangeProductSupplierCommandResult>;

internal record ChangeProductSupplierCommandResult;

internal class ChangeProductSupplierCommandHandler :
    ICommandHandler<ChangeProductSupplierCommand, ChangeProductSupplierCommandResult>
{
    public Task<ChangeProductSupplierCommandResult> Handle(
        ChangeProductSupplierCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
