using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.UpdatingProduct;

internal record UpdateProductCommand : ICommand<UpdateProductCommandResult>;

internal record UpdateProductCommandResult;

internal class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, UpdateProductCommandResult>
{
    public Task<UpdateProductCommandResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
