using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;

namespace Catalog.Products.Features.DebitingProductStock;

public record DebitProductStock(long ProductId, int Quantity) : ICommand<bool>;

internal class DebitProductStockHandler : ICommandHandler<DebitProductStock, bool>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public DebitProductStockHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
    }

    public async Task<bool> Handle(DebitProductStock command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var product = await _catalogDbContext.FindProductAsync(command.ProductId, cancellationToken);
        Guard.Against.NullProduct(product, command.ProductId);
        product!.DebitStock(command.Quantity);

        await _catalogDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
