using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products.Features.CreatingProduct;

namespace ECommerce.Services.Catalogs.Products.Features.ReplenishingProductStock;


// POST api/v1/catalog/products/{productId}/replenish-stock
public static class ReplenishingProductStockEndpoint
{
    internal static IEndpointRouteBuilder MapReplenishProductStockEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                $"{ProductsConfigs.ProductsPrefixUri}/{{productId}}/replenish-stock",
                ReplenishProductStock)
            .WithTags(ProductsConfigs.Tag)
            .Produces<CreateProductResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DebitProductStock")
            .WithDisplayName("Debit product stock");

        return endpoints;
    }

    private static async Task<IResult> ReplenishProductStock(
        long productId,
        int quantity,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var result = await commandProcessor.SendAsync(new ReplenishingProductStock(productId, quantity), cancellationToken);

        return Results.Ok(result);
    }
}
