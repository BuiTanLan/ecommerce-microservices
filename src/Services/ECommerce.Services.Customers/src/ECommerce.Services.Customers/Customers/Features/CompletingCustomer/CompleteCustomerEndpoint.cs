using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Web.MinimalApi;

namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer;

public class CompleteCustomerEndpoint : IMinimalEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/complete-profile", CompleteCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Complete customer profile.");

        return builder;
    }

    private static async Task<IResult> CompleteCustomer(
        long customerId,
        CompleteCustomerRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CompleteCustomer(
            customerId,
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
