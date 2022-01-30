using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Exceptions;

public class UserLockedException : BadRequestException
{
    public UserLockedException(string userId) : base($"userId '{userId}' has been locked.")
    {
    }
}
