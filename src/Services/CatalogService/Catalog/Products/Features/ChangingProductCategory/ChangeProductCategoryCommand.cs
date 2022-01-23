using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.ChangingProductCategory;

internal record ChangeProductCategoryCommand : ICommand<ChangeProductCategoryCommandResult>;

internal record ChangeProductCategoryCommandResult;

internal class ChangeProductCategoryCommandHandler :
    ICommandHandler<ChangeProductCategoryCommand, ChangeProductCategoryCommandResult>
{
    public Task<ChangeProductCategoryCommandResult> Handle(
        ChangeProductCategoryCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
