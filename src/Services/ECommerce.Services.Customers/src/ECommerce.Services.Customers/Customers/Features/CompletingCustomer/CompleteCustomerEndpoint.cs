using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;

namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer;

public static class CompleteCustomerEndpoint
{
    internal static IEndpointRouteBuilder MapCompleteCustomerEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{CustomersConfigs.CustomersPrefixUri}/complete-profile", CompleteCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Complete customer profile.");

        return endpoints;
    }

    private static async Task<IResult> CompleteCustomer(
        CompleteCustomerRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CompleteCustomer(
            request.CustomerId,
            request.PhoneNumber,
            request.BirthDate,
            request.Country,
            request.City,
            request.DetailAddress,
            request.Nationality);

        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
