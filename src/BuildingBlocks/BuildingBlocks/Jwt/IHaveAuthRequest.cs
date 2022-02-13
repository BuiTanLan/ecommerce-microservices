using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Jwt;

public interface IHaveAuthRequest : IAuthorizationRequirement
{
}
