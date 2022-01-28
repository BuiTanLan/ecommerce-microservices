using BuildingBlocks.CQRS.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Features.GetClaims;

public static class GetClaimsEndpoint
{
    private const string Tag = "ECommerce.Services.Identity";

    internal static IEndpointRouteBuilder MapGetClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{IdentityConfiguration.IdentityModulePrefixUri}/claims", GetClaims)
            .WithTags(Tag)
            .RequireAuthorization()
            .Produces<GetClaimsQueryResult>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithDisplayName("Get IdentityUser claims");

        return endpoints;
    }

    private static async Task<IResult> GetClaims(
        IQueryProcessor queryProcessor, CancellationToken cancellationToken)
    {
        var result = await queryProcessor.SendAsync(new GetClaimsQuery(), cancellationToken);

        return Results.Ok(result);
    }
}
