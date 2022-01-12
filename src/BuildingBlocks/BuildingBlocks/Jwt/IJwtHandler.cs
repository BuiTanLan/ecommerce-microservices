using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Jwt
{
    public interface IJwtHandler
    {
        public JsonWebToken GenerateJwtToken(
            string userName,
            string email,
            string userId,
            bool? isVerified = null,
            string fullName = null,
            string refreshToken = null,
            IList<Claim> usersClaims = null,
            IList<string> rolesClaims = null,
            IList<string> permissionsClaims = null);

        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
        JsonWebTokenPayload GetTokenPayload(string accessToken);
    }
}
