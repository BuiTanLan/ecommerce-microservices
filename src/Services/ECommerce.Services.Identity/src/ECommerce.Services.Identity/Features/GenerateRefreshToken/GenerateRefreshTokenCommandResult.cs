using ECommerce.Services.Identity.Share.Core.Dtos;

namespace ECommerce.Services.Identity.Features.GenerateRefreshToken;

public record GenerateRefreshTokenCommandResult(RefreshTokenDto RefreshToken);
