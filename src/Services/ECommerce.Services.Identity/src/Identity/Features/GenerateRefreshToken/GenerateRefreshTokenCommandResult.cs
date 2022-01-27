using Identity.Share.Core.Dtos;

namespace Identity.Features.GenerateRefreshToken;

public record GenerateRefreshTokenCommandResult(RefreshTokenDto RefreshToken);
