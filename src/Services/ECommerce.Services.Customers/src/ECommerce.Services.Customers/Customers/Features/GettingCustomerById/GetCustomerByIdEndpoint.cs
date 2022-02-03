using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Catalogs.Products;

namespace ECommerce.Services.Customers.Customers.Features.GettingCustomerById;


public static class GetCustomerByIdEndpointEndpoint
{
    internal static IEndpointRouteBuilder MapGetCustomerByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                $"{CustomersConfigs.CustomersPrefixUri}/{{id}}",
                GetCustomerById)
            .WithTags(CustomersConfigs.Tag)
            // .RequireAuthorization()
            .Produces<GetCustomerByIdResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetCustomerById")
            .WithDisplayName("Get Customer By Id.");

        return endpoints;
    }

    private static async Task<IResult> GetCustomerById(
        long id,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(id, nameof(id));

        var result = await queryProcessor.SendAsync(new GetCustomerById(id), cancellationToken);

        return Results.Ok(result);
    }
}
