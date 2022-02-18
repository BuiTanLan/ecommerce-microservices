using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Web.MinimalApi;

namespace ECommerce.Services.Customers.Customers.Features.LockingCustomer;

public class LockCustomerEndpoint : IMinimalEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/lock", LockCustomer)
            .RequireAuthorization(CustomersConstants.Role.Admin)
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName("Lock Customer.");

        return builder;
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
