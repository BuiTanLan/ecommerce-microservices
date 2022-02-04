using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Catalogs.Products;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomerWithIdentity;

public static class CreateCustomerWIthIdentityEndpoint
{
    internal static IEndpointRouteBuilder MapCreateCustomerWIthIdentityEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{CustomersConfigs.CustomersPrefixUri}/create-with-identity", CreateCustomerWithIdentity)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces<CreateCustomerWithIdentityResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateCustomerWithIdentity")
            .WithDisplayName("Register New Customer with identity information.");

        return endpoints;
    }

    private static async Task<IResult> CreateCustomerWithIdentity(
        CreateCustomerWithIdentityRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateCustomerWIthIdentity(
            request.UserName,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{CustomersConfigs.CustomersPrefixUri}/{result.CustomerId}", result);
    }
}
