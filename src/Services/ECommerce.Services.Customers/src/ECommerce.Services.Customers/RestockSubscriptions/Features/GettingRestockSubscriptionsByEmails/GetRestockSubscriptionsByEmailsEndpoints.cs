using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionsByEmails;

public static class GetRestockSubscriptionsByEmailsEndpoints
{
    internal static IEndpointRouteBuilder MapGetRestockSubscriptionsByEmailsEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                $"{RestockSubscriptionsConfigs.RestockSubscriptionsUrl}/by-emails",
                GetRestockSubscriptionsByEmails)
            .WithTags(RestockSubscriptionsConfigs.Tag)
            // .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetRestockSubscriptionsByEmails")
            .WithDisplayName("Get Restock Subscriptions by emails.");

        return endpoints;
    }

    private static IAsyncEnumerable<RestockSubscriptionDto> GetRestockSubscriptionsByEmails(
        GetRestockSubscriptionsByEmailsRequest? request,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = queryProcessor.SendAsync(new GetRestockSubscriptionsByEmails(request.Emails), cancellationToken);

        return result;
    }
}
