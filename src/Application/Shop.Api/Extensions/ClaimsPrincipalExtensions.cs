namespace Shop.Api.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetClaimValue(this ClaimsPrincipal principal, string type)
        => principal.FindFirst(type)!.Value;
}