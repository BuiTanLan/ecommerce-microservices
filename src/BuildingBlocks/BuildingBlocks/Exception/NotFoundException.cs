namespace BuildingBlocks.Exception;

public class NotFoundException : AppException
{
    public NotFoundException(string message, string code = null) : base(message, code)
    {
    }
}
