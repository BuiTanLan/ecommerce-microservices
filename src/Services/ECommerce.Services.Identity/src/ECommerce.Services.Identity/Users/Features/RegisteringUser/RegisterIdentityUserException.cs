using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Users.Features.RegisteringUser;

public class RegisterIdentityUserException : BadRequestException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
