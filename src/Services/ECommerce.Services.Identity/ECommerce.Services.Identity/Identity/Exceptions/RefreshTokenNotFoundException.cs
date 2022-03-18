using MicroBootstrap.Core.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Exceptions;

public class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base("Refresh token not found.")
    {
    }
}
