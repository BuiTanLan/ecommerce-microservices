namespace Catalog;

public static class CatalogConstants
{
    public static string LogTemplate => "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}";

    public static string DevelopmentLogPath => "../Log/e-commerce.log";

    public static string ProductionLogPath => "./Log/e-commerce.log";

    public static string IdentityRoleName => "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
}

