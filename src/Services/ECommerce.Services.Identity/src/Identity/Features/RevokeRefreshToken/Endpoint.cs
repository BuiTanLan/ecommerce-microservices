using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Features.RevokeRefreshToken;

public static class Endpoint
{
    private const string Tag = "Identity";

    internal static IEndpointRouteBuilder MapRevokeTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/revoke-refresh-token", RevokeToken)
            .WithTags(Tag)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Refresh Token.");

        return endpoints;
    }

    private static async Task<IResult> RevokeToken(RevokeRefreshTokenRequest request,
        ICommandProcessor commandProcessor, CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokenCommand(request.RefreshToken);

        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.NoContent();
    }
}
