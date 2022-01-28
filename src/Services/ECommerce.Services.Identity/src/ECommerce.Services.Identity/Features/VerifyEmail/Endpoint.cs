using BuildingBlocks.CQRS.Command;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Features.VerifyEmail;

public static class Endpoint
{
    private const string Tag = "ECommerce.Services.Identity";

    internal static IEndpointRouteBuilder MapSendVerifyEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                $"{IdentityConfiguration.IdentityModulePrefixUri}/verify-email",
                VerifyEmail)
            .WithTags(Tag)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDisplayName("Verify Email.");

        return endpoints;
    }

    private static async Task<IResult> VerifyEmail(
        VerifyEmailRequest request,
        ICommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var command = new VerifyEmailCommand(request.Email, request.Code);

        await commandProcessor.SendAsync(command, cancellationToken);

        return Results.Ok();
    }
}
