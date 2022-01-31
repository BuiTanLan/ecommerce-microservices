using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Catalogs.Customers.Features.GettingCustomers;

namespace ECommerce.Services.Catalogs.Products.Features.GettingProducts;

// GET api/v1/catalog/products
public static class GetCustomersEndpoint
{
    internal static IEndpointRouteBuilder MapGetCustomersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{CustomersConfigs.CustomersPrefixUri}", GetCustomers)
            .WithTags(CustomersConfigs.Tag)
            // .RequireAuthorization()
            .Produces<GetCustomersResult>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetCustomers")
            .WithDisplayName("Get customers.");

        return endpoints;
    }

    private static async Task<IResult> GetCustomers(
        GetCustomersRequest? request,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = await queryProcessor.SendAsync(
            new GetCustomers(
                request.Page,
                request.PageSize,
                request.CustomerState,
                request.Includes,
                request.Filters,
                request.Sorts),
            cancellationToken);

        return Results.Ok(result);
    }
}
