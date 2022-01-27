using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Features.Login;

public static class LoginEndpoint
{
    private const string Tag = "Identity";

    internal static IEndpointRouteBuilder MapLoginUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/login", LoginUser)
            .AllowAnonymous()
            .WithTags(Tag)
            .Produces<LoginCommandResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Login User.");

        return endpoints;
    }

    private static async Task<IResult> LoginUser(LoginUserRequest request,
        ICommandProcessor commandProcessor, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.UserNameOrEmail, request.Password, request.Remember);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Ok(result);
    }
}
