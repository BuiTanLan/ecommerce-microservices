using System.Collections.Generic;
using System.Net;

namespace BuildingBlocks.Exception
{
    public class CustomException : System.Exception
    {
        public List<string> ErrorMessages { get; } = new();

        public HttpStatusCode StatusCode { get; }

        public CustomException(string message, List<string> errors = default,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            ErrorMessages = errors;
            StatusCode = statusCode;
        }
    }
}
