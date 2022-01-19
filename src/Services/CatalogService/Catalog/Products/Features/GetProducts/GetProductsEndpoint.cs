using BuildingBlocks.CQRS.Query;
using BuildingBlocks.Web.Extensions;

namespace Catalog.Products.Features.GetProducts;

public static class GetProductsEndpoint
{
    private const string Tag = "Catalog";

    internal static IEndpointRouteBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{CatalogConfiguration.CatalogModulePrefixUri}/products", GetProducts)
            .WithTags(Tag)
            // .RequireAuthorization()
            .Produces<GetProductsQueryResult>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Get products.");

        return endpoints;
    }

    private static async Task<IResult> GetProducts(
        [FromHeader(Name = "x-query")] string xQuery,
        HttpContext httpContext,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        var request = httpContext.SafeGetListQuery<GetProductRequest>(xQuery);
        if (request is null)
            return Results.BadRequest();

        var result = await queryProcessor.SendAsync(
            new GetProductsQuery
            {
                Filters = request.Filters,
                Page = request.Page,
                Sorts = request.Sorts,
                PageSize = request.PageSize,
            },
            cancellationToken);

        return Results.Ok(result);
    }
}
