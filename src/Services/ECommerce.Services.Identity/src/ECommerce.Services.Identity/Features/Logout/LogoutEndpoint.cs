using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Features.Logout;

public static class LogoutEndpoint
{
    private const string Tag = "ECommerce.Services.Identity";

    internal static IEndpointRouteBuilder MapLogoutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{IdentityConfiguration.IdentityModulePrefixUri}/logout", async (HttpContext httpContext) =>
            {
                await httpContext.SignOutAsync();
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .WithTags(Tag)
            .RequireAuthorization()
            .WithDisplayName("Logout ECommerce.Services.Identity User.");

        return endpoints;
    }
}
