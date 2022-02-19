using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Web.MinimalApi;

namespace ECommerce.Services.Customers.Customers.Features.UnlockingCustomer;

public class UnlockCustomerEndpoint : IMinimalEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/unlock", UnlockCustomer)
            .RequireAuthorization(CustomersConstants.Role.Admin)
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName("Unlock Customer.");

        return builder;
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
