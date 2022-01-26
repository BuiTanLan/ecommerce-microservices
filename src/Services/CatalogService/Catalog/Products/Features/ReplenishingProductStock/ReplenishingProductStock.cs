using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;

namespace Catalog.Products.Features.ReplenishingProductStock;

public record ReplenishingProductStock(long ProductId, int Quantity) : ICommand<bool>;

internal class ReplenishingProductStockHandler : ICommandHandler<ReplenishingProductStock, bool>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ReplenishingProductStockHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
    }

    public async Task<bool> Handle(ReplenishingProductStock command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var product = await _catalogDbContext.FindProductAsync(command.ProductId, cancellationToken);
        Guard.Against.NullProduct(product, command.ProductId);

        product!.ReplenishStock(command.Quantity);
        await _catalogDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
