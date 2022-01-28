using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Features.RefreshToken;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() : base("access_token is invalid!")
    {
    }
}
