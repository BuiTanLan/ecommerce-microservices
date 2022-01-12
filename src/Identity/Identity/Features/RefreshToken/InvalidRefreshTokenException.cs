using BuildingBlocks.Exception;

namespace Identity.Features.RefreshToken;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() : base("access_token is invalid!")
    {
    }
}
