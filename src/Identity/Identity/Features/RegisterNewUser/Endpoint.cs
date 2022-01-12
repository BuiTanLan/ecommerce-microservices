using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Features.RegisterNewUser;

public static class Endpoint
{
    private const string Tag = "Identity";

    internal static IEndpointRouteBuilder MapRegisterNewUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/register", RegisterUser)
            .AllowAnonymous()
            .WithTags(Tag)
            .Produces<RegisterNewUserCommandResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Register New IdentityUser");

        return endpoints;
    }

    private static async Task<IResult> RegisterUser(RegisterNewUserRequest request,
        ICommandProcessor commandProcessor, CancellationToken cancellationToken)
    {
        var command = new RegisterNewUserCommand(request.FirstName, request.LastName,
            request.UserName, request.Email, request.Password, request.ConfirmPassword);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{IdentityConfiguration.IdentityModulePrefixUri}/{result.User.Id}", result);
    }
}
