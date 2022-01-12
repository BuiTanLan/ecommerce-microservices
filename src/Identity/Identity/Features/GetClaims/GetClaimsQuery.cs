using BuildingBlocks.CQRS.Query;
using Microsoft.AspNetCore.Http;

namespace Identity.Features.GetClaims;

public class GetClaimsQuery : IQuery<Dictionary<string, string>>
{
}

public class GetClaimsQueryHandler : IQueryHandler<GetClaimsQuery, Dictionary<string, string>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetClaimsQueryHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Dictionary<string, string>> Handle(GetClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = _httpContextAccessor.HttpContext?.User.Claims;

        return Task.FromResult(claims?.ToDictionary(x => x.Type, x => x.Value));
    }
}
