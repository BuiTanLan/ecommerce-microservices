namespace BuildingBlocks.EFCore;

public static class Constants
{
    public const string UuidGenerator = "uuid-ossp";
    public const string UuidAlgorithm = "uuid_generate_v4()";
    public const string DateAlgorithm = "now()";
    public const string PriceDecimal = "decimal(18,2)";
    public const string ShortText = "varchar(25)";
    public const string NormalText = "varchar(50)";
    public const string LongText = "varchar(250)";
    public const string ExtraLongText = "varchar(500)";
}
