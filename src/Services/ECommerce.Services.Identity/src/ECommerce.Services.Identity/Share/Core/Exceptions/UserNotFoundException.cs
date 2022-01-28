using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Share.Core.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string userNameOrEmail) : base(
        $"User with Username or email '{userNameOrEmail}' not found.")
    {
    }
}
