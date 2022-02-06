using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;

namespace ECommerce.Services.Customers.Customers.Features.GettingCustomers;

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
            new GetCustomers
            {
                Filters = request.Filters,
                Includes = request.Includes,
                Page = request.Page,
                Sorts = request.Sorts,
                CustomerState = request.CustomerState,
                PageSize = request.PageSize
            },
            cancellationToken);

        return Results.Ok(result);
    }
}
