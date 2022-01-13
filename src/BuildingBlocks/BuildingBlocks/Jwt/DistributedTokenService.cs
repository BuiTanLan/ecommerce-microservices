using System;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Web.Extensions;
using EasyCaching.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Jwt;

public class DistributedTokenService : IAccessTokenService
{
    private readonly IEasyCachingProvider _cacheProvider;
    private readonly TimeSpan _expires;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DistributedTokenService(IEasyCachingProviderFactory cachingFactory,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions)
    {
        _cacheProvider = cachingFactory.GetCachingProvider("redis");
        _httpContextAccessor = httpContextAccessor;
        _expires = TimeSpan.FromMinutes(jwtOptions.Value?.ExpiryMinutes ?? 120);
    }

    public Task<bool> IsCurrentActiveToken()
    {
        return IsActiveAsync(GetCurrent());
    }

    public Task DeactivateCurrentAsync()
    {
        return DeactivateAsync(GetCurrent());
    }

    public async Task<bool> IsActiveAsync(string token)
    {
        return string.IsNullOrWhiteSpace((await _cacheProvider.GetAsync<string>(GetKey(token))).Value);
    }

    public Task DeactivateAsync(string token)
    {
        return _cacheProvider.SetAsync(GetKey(token), "revoked", _expires);
    }

    private string GetCurrent()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers.Get<string>("authorization");

        return authorizationHeader is null || authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Split(' ').Last();
    }

    private static string GetKey(string token)
    {
        return $"blacklisted-tokens:{token}";
    }
}
