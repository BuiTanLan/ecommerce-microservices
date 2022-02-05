using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Services.Identity.Users.Features.GettingUsers;

public static class GetUsersEndpoint
{
    internal static IEndpointRouteBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{UsersConfigs.UsersPrefixUri}", GetUsers)
            .WithTags(UsersConfigs.Tag)
            // .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetUsers")
            .WithDisplayName("Get users.");

        return endpoints;
    }

    private static async Task<IResult> GetUsers(
        GetUsersRequest? request,
        IQueryProcessor queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = await queryProcessor.SendAsync(
            new GetUsers
            {
                Filters = request.Filters,
                Includes = request.Includes,
                Page = request.Page,
                Sorts = request.Sorts,
                PageSize = request.PageSize
            },
            cancellationToken);

        return Results.Ok(result);
    }
}
