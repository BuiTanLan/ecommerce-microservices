using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Features.RefreshToken;

public static class RefreshTokenEndpoint
{
    private const string Tag = "ECommerce.Services.Identity";

    internal static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/refresh-token", RefreshToken)
            .WithTags(Tag)
            .RequireAuthorization()
            .Produces<RefreshTokenCommandResult>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Refresh Token.");

        return endpoints;
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);

        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Ok(result);
    }
}
