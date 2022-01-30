using System.Net;

namespace BuildingBlocks.Exception.Types;

public class AppException : CustomException
{
    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message)
    {
        StatusCode = statusCode;
    }
}
