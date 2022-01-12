using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Ardalis.GuardClauses;
using BuildingBlocks.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Jwt;

public sealed class JwtHandler : IJwtHandler
{
    private static readonly ISet<string> s_defaultClaims = new HashSet<string>
    {
        JwtRegisteredClaimNames.Sub,
        JwtRegisteredClaimNames.UniqueName,
        JwtRegisteredClaimNames.Jti,
        JwtRegisteredClaimNames.Iat,
        ClaimTypes.Role
    };

    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly JwtOptions _options;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ILogger<JwtHandler> _logger;

    public JwtHandler(IOptions<JwtOptions> options,
        TokenValidationParameters tokenValidationParameters,
        ILogger<JwtHandler> logger)
    {
        _options = Guard.Against.Null(options?.Value, nameof(options));
        _tokenValidationParameters = tokenValidationParameters;
        _logger = logger;
    }

    public JsonWebToken GenerateJwtToken(
        string userName,
        string email,
        string userId,
        bool? isVerified = null,
        string fullName = null,
        string refreshToken = null,
        IList<Claim> usersClaims = null,
        IList<string> rolesClaims = null,
        IList<string> permissionsClaims = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User ID claim (subject) cannot be empty.", nameof(userName));

        var now = DateTime.Now;
        string ipAddress = IpHelper.GetIpAddress();

        //https://leastprivilege.com/2017/11/15/missing-claims-in-the-asp-net-core-2-openid-connect-handler/
        //https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/a301921ff5904b2fe084c38e41c969f4b2166bcb/src/System.IdentityModel.Tokens.Jwt/ClaimTypeMapping.cs#L45-L125
        //https://stackoverflow.com/a/50012477/581476
        List<Claim> jwtClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, userId),
            new Claim(JwtRegisteredClaimNames.Name, fullName ?? ""),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Sid, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.GivenName, fullName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)),
            new Claim("rt", refreshToken ?? ""),
            new("ip", ipAddress)
        };

        if (rolesClaims?.Any() is true)
        {
            foreach (var role in rolesClaims)
            {
                jwtClaims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (!string.IsNullOrWhiteSpace(_options.Audience))
        {
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _options.Audience));
        }

        if (permissionsClaims?.Any() is true)
        {
            foreach (var permissionsClaim in permissionsClaims)
            {
                jwtClaims.Add(new Claim(CustomClaimTypes.Permission, permissionsClaim));
            }
        }

        if (usersClaims?.Any() is true)
        {
            jwtClaims = jwtClaims.Union(usersClaims).ToList();
        }

        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
        if (issuerSigningKey is null)
            throw new InvalidOperationException("Issuer signing key not set.");
        var signingCredentials =
            new SigningCredentials(issuerSigningKey, _options?.Algorithm ?? SecurityAlgorithms.HmacSha256);

        var expire = now.AddMinutes(_options?.ExpiryMinutes ?? 120);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            notBefore: now,
            claims: jwtClaims,
            expires: expire,
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);


        return new JsonWebToken
        {
            IsVerified = isVerified,
            AccessToken = token,
            Expires = expire,
            UserId = userId,
            Email = email,
            Roles = rolesClaims?.ToList() ?? Enumerable.Empty<string>().ToList(),
            Permissions = permissionsClaims?.ToList() ?? Enumerable.Empty<string>().ToList()
        };
    }

    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
    {
        try
        {
            var principal =
                _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals
                    (_options.Algorithm ?? SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        catch (System.Exception e)
        {
            _logger.LogError("Token validation failed: {Message}", e.Message);
            return null;
        }
    }

    public JsonWebTokenPayload GetTokenPayload(string accessToken)
    {
        _jwtSecurityTokenHandler.ValidateToken(accessToken, _tokenValidationParameters,
            out var validatedSecurityToken);
        if (!(validatedSecurityToken is JwtSecurityToken jwt))
            return null;

        return new JsonWebTokenPayload
        {
            Subject = jwt.Subject,
            Role = jwt.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
            Expires = jwt.ValidTo.ToUnixTimeMilliseconds(),
            Claims = jwt.Claims.Where(x => !s_defaultClaims.Contains(x.Type))
                .GroupBy(c => c.Type)
                .ToDictionary(k => k.Key, v => v.Select(c => c.Value))
        };
    }
}
