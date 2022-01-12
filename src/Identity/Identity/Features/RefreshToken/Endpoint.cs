using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Features.RefreshToken;

public static class Endpoint
{
    private const string Tag = "Identity";

    internal static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/refresh-token",
                LoginUser)
            .AllowAnonymous()
            .WithTags(Tag)
            .Produces<RefreshTokenCommandResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Refresh Token.");

        return endpoints;
    }

    private static async Task<IResult> LoginUser(RefreshTokenRequest request,
        ICommandProcessor commandProcessor, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Ok(result);
    }
}
