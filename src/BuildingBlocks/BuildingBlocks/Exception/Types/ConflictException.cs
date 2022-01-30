using System.Net;

namespace BuildingBlocks.Exception.Types;

public class ConflictException : CustomException
{
    public ConflictException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
}
