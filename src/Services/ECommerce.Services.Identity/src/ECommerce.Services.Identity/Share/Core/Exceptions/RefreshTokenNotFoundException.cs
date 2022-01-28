using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Share.Core.Exceptions;

public class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base("Refresh token not found.")
    {
    }
}
