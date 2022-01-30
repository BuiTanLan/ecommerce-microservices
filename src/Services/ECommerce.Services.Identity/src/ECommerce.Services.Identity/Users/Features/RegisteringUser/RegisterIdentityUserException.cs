using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Users.Features.RegisteringUser;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
