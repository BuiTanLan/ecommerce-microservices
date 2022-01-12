using BuildingBlocks.Exception;

namespace Identity.Core.Exceptions;

public class InvalidTokenException : BadRequestException
{
    public InvalidTokenException() : base("access_token is invalid!")
    {
    }
}
