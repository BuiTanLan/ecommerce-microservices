using Identity.Core.Dtos;

namespace Identity.Features.GenerateRefreshToken;

public record GenerateRefreshTokenCommandResult(RefreshTokenDto RefreshToken);
