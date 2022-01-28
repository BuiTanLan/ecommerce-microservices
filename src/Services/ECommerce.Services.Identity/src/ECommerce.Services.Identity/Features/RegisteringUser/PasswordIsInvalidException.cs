using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Features.RegisteringUser;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
