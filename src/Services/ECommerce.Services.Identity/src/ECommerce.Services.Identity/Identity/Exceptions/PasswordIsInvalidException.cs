using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Exceptions;

public class PasswordIsInvalidException : AppException
{
    public PasswordIsInvalidException(string message) : base(message)
    {
    }
}
