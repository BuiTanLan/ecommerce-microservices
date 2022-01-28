using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Share.Core.Exceptions;

public class UserLockedException : BadRequestException
{
    public UserLockedException(string userId) : base($"userId '{userId}' has been locked.")
    {
    }
}
