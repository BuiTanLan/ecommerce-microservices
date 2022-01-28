using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products.Features.CreatingProduct;

namespace ECommerce.Services.Catalogs.Products.Features.DebitingProductStock;

// POST api/v1/catalog/products/{productId}/debit-stock
public static class DebitProductStockEndpoint
{
    internal static IEndpointRouteBuilder MapDebitProductStockEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                $"{CatalogConfiguration.CatalogModulePrefixUri}{ProductsConfigs.ProductsPrefixUri}/{{productId}}/debit-stock",
                DebitProductStock)
            .WithTags(ProductsConfigs.Tag)
            .Produces<CreateProductResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DebitProductStock")
            .WithDisplayName("Debit product stock");

        return endpoints;
    }

    private static async Task<IResult> DebitProductStock(
        long productId,
        int quantity,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var result = await commandProcessor.SendAsync(new DebitProductStock(productId, quantity), cancellationToken);

        return Results.Ok(result);
    }
}
