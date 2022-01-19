using BuildingBlocks.CQRS.Query;

namespace Catalog.Products.Features.GetProductsView;

public static class GetProductsViewEndpoint
{
    private const string Tag = "Catalog";

    internal static IEndpointRouteBuilder MapGetProductsViewEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                $"{CatalogConfiguration.CatalogModulePrefixUri}/products-view/{{page}}/{{pageSize}}",
                GetProductsView)
            .WithTags(Tag)
            // .RequireAuthorization()
            .Produces<GetProductsViewQueryResult>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Get products.");

        return endpoints;
    }

    private static async Task<IResult> GetProductsView(
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 20)
    {
        var result = await queryProcessor.SendAsync(
            new GetProductsViewQuery { Page = page, PageSize = pageSize },
            cancellationToken);

        return Results.Ok(result);
    }
}
