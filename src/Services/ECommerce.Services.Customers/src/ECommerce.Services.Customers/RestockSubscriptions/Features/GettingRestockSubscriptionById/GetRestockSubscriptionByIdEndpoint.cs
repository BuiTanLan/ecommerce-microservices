using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Customers.RestockSubscriptions;
using ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionById;

namespace ECommerce.Services.Catalogs.Products.Features.GettingProductById;

public static class GetRestockSubscriptionByIdEndpoint
{
    internal static IEndpointRouteBuilder MapGetRestockSubscriptionByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                $"{RestockSubscriptionsConfigs.RestockSubscriptionsUrl}/{{id}}",
                GetRestockSubscriptionById)
            .WithTags(RestockSubscriptionsConfigs.Tag)
            // .RequireAuthorization()
            .Produces<GetRestockSubscriptionByIdResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetRestockSubscriptionById")
            .WithDisplayName("Get RestockSubscription By Id.");

        return endpoints;
    }

    private static async Task<IResult> GetRestockSubscriptionById(
        long id,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(id, nameof(id));

        var result = await queryProcessor.SendAsync(new GetRestockSubscriptionById(id), cancellationToken);

        return Results.Ok(result);
    }
}
