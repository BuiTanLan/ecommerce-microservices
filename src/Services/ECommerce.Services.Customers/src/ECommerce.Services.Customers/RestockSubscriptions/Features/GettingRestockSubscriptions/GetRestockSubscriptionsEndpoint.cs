using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptions;

public static class GetRestockSubscriptionsEndpoint
{
    internal static IEndpointRouteBuilder MapGetRestockSubscriptionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{RestockSubscriptionsConfigs.RestockSubscriptionsUrl}", GetRestockSubscriptions)
            .WithTags(RestockSubscriptionsConfigs.Tag)
            // .RequireAuthorization()
            .Produces<GetRestockSubscriptionsResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetRestockSubscriptions")
            .WithDisplayName("Get Restock Subscriptions.");

        return endpoints;
    }

    private static async Task<IResult> GetRestockSubscriptions(
        GetRestockSubscriptionsRequest? request,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = await queryProcessor.SendAsync(
            new GetRestockSubscriptions
            {
                Page = request.Page,
                Sorts = request.Sorts,
                PageSize = request.PageSize,
                Filters = request.Filters,
                Includes = request.Includes,
                Emails = request.Emails,
                From = request.From,
                To = request.To
            },
            cancellationToken);

        return Results.Ok(result);
    }
}
