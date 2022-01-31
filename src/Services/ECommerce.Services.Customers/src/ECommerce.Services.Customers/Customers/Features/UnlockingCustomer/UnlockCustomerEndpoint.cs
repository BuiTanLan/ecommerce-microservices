using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products;

namespace ECommerce.Services.Customers.Customers.Features.UnlockingCustomer;

public static class UnlockCustomerEndpoint
{
    internal static IEndpointRouteBuilder MapUnlockCustomerEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/unlock", UnlockCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName("Unlock Customer.");

        return endpoints;
    }

    private static async Task<IResult> UnlockCustomer(
        long customerId,
        UnlockCustomerRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new UnlockCustomer(customerId, request.Notes);
        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
