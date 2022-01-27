using BuildingBlocks.Exception;

namespace Identity.Share.Core.Exceptions;

public class RefreshTokenNotFoundException : NotFoundException
{
    public RefreshTokenNotFoundException() : base("Refresh token not found.")
    {
    }
}
