using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;

namespace ECommerce.Services.Customers.Customers.Features.LockingCustomer;

public static class LockCustomerEndpoint
{
    internal static IEndpointRouteBuilder MapLockCustomerEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/lock", LockCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName("Lock Customer.");

        return endpoints;
    }

    private static async Task<IResult> LockCustomer(
        long customerId,
        LockCustomerRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new LockCustomer(customerId, request.Notes);
        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
