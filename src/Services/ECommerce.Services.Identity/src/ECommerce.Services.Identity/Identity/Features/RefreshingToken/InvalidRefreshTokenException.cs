using MicroBootstrap.Core.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Features.RefreshingToken;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() : base("access_token is invalid!")
    {
    }
}
