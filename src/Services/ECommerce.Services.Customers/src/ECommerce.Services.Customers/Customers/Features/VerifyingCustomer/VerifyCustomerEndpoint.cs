using MicroBootstrap.Abstractions.CQRS.Command;
using MicroBootstrap.Web.MinimalApi;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer;

public class VerifyCustomerEndpoint : IMinimalEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{CustomersConfigs.CustomersPrefixUri}/{{customerId}}/verify", VerifyCustomer)
            .AllowAnonymous()
            .WithTags(CustomersConfigs.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName("Verify Customer.");

        return builder;
    }

    private static async Task<IResult> VerifyCustomer(
        [FromRoute]long customerId,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var command = new VerifyCustomer(customerId);
        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
