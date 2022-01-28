using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Features.RegisteringUser;

public static class RegisterNewUserEndpoint
{
    private const string Tag = "ECommerce.Services.Identity";

    internal static IEndpointRouteBuilder MapRegisterNewUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/register", RegisterUser)
            .AllowAnonymous()
            .WithTags(Tag)
            .Produces<RegisterNewUserResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Register New IdentityUser");

        return endpoints;
    }

    private static async Task<IResult> RegisterUser(
        RegisterNewUserRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var command = new RegisterNewUser(
            request.FirstName,
            request.LastName,
            request.UserName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.Roles?.ToList());

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Created($"{IdentityConfiguration.IdentityModulePrefixUri}/{result.User.Id}", result);
    }
}
