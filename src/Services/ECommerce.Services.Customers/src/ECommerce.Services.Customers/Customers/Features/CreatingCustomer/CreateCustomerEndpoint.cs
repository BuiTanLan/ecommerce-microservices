using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

public static class CreateCustomerEndpoint
{
    internal static IEndpointRouteBuilder MapCreateCustomerEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(CustomersConfigs.CustomersPrefixUri, RegisterCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces<CreateCustomerResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Register New Customer.");

        return endpoints;
    }

    private static async Task<IResult> RegisterCustomer(
        CreateCustomerRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateCustomer(
            request.UserName,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{CustomersConfigs.CustomersPrefixUri}/{result.CustomerId}", result);
    }
}
