using System.Net;

namespace BuildingBlocks.Exception.Types;

public class BadRequestException : CustomException
{
    public BadRequestException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}
