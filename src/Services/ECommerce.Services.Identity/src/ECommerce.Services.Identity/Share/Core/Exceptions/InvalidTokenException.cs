using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Share.Core.Exceptions;

public class InvalidTokenException : BadRequestException
{
    public InvalidTokenException() : base("access_token is invalid!")
    {
    }
}
