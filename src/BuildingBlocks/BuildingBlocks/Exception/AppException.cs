namespace BuildingBlocks.Exception;

public class AppException : System.Exception
{
    public AppException(string message, string code = default!) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
