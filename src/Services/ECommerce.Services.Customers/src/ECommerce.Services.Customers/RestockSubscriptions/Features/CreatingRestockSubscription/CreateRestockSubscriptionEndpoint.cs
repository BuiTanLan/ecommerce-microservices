using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription;

public static class CreateRestockSubscriptionEndpoint
{
    internal static IEndpointRouteBuilder MapCreateRestockSubscriptionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(RestockSubscriptionsConfigs.RestockSubscriptionsUrl, CreateRestockSubscription)
            .AllowAnonymous()
            .WithTags(RestockSubscriptionsConfigs.Tag)
            .Produces<CreateRestockSubscriptionResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("CreateRestockSubscription")
            .WithDisplayName("Register New RestockSubscription for Customer.");

        return endpoints;
    }

    private static async Task<IResult> CreateRestockSubscription(
        CreateRestockSubscriptionRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateRestockSubscription(request.CustomerId, request.ProductId, request.Email);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{RestockSubscriptionsConfigs.RestockSubscriptionsUrl}/{result.RestockSubscription.Id}", result);
    }
}
