using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Identity.Users.Features.RegisteringUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Users.Features.GettingUerByEmail;

public static class GetUserByEmailEndpoint
{
    internal static IEndpointRouteBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{UsersConfigs.UsersPrefixUri}/{{email}}", GetUserByEmail)
            .AllowAnonymous()
            .WithTags(UsersConfigs.Tag)
            .Produces<RegisterUserResult>(StatusCodes.Status200OK)
            .Produces<RegisterUserResult>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetUserByEmail")
            .WithDisplayName("Get User by email.");

        return endpoints;
    }

    private static async Task<IResult> GetUserByEmail(
        string email,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        var result = await queryProcessor.SendAsync(new GetUserByEmail(email), cancellationToken);

        return Results.Ok(result);
    }
}
