using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.UpdatingProduct;

internal record UpdateProduct : ICommand<UpdateProductCommand>;

internal record UpdateProductCommand;

internal class UpdateProductCommandHandler : ICommandHandler<UpdateProduct, UpdateProductCommand>
{
    public Task<UpdateProductCommand> Handle(UpdateProduct request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
