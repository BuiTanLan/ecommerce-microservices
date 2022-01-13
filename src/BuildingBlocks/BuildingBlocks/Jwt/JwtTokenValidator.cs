using System;
using System.Security.Claims;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Jwt;

public class JwtTokenValidator : IJwtTokenValidator
{
    private readonly IJwtHandler _jwtTokenHandler;
    private readonly JwtOptions _options;

    public JwtTokenValidator(IJwtHandler jwtTokenHandler, IOptions<JwtOptions> options)
    {
        _jwtTokenHandler = jwtTokenHandler;
        _options = _options = Guard.Against.Null(options?.Value, nameof(options));
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
        if (issuerSigningKey is null)
            throw new InvalidOperationException("Issuer signing key not set.");

        return _jwtTokenHandler.ValidateToken(token,
            new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                ValidateLifetime = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            });
    }
}
