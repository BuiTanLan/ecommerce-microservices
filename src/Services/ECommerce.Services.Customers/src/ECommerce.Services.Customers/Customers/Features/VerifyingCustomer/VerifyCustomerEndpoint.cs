using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer;

public static class VerifyCustomerEndpoint
{
    internal static IEndpointRouteBuilder MapVerifyCustomerEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/verify", VerifyCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName("Verify Customer.");

        return endpoints;
    }

    private static async Task<IResult> VerifyCustomer(
        long customerId,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var command = new VerifyCustomer(customerId);
        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
