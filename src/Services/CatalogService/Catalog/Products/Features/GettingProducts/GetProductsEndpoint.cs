using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;

namespace Catalog.Products.Features.GettingProducts;

// GET api/v1/catalog/products
public static class GetProductsEndpoint
{
    internal static IEndpointRouteBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{CatalogConfiguration.CatalogModulePrefixUri}{ProductsConfigs.ProductsPrefixUri}", GetProducts)
            .WithTags(ProductsConfigs.Tag)
            // .RequireAuthorization()
            .Produces<GetProductsResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetProducts")
            .WithDisplayName("Get products.");

        return endpoints;
    }

    private static async Task<IResult> GetProducts(
            GetProductRequest? request,
            IQueryProcessor queryProcessor,
            CancellationToken cancellationToken)

        // [FromHeader(Name = "x-query")] string xQuery,)
    {
        // var request = httpContext.ExtractXQueryObjectFromHeader<GetProductRequest>(xQuery);
        Guard.Against.Null(request, nameof(request));

        var result = await queryProcessor.SendAsync(
            new GetProducts
            {
                Page = request.Page,
                Sorts = request.Sorts,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Includes = request.Includes,
            },
            cancellationToken);

        return Results.Ok(result);
    }
}
