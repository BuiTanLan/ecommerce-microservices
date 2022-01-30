using System.Net;

namespace BuildingBlocks.Exception.Types;

public class NotFoundException : CustomException
{
    public NotFoundException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}
