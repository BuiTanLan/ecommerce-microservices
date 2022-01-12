using System.Security.Claims;

namespace BuildingBlocks.Jwt
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
