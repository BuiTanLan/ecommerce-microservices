using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Features.Login.Exceptions;

public class PasswordIsInvalidException : BadRequestException
{
    public PasswordIsInvalidException() : base("password is invalid!")
    {
    }
}
