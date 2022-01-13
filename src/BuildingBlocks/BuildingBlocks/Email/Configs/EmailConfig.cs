namespace BuildingBlocks.Email.Configs;

public class EmailConfig
{
    public MimeKitConfig MimeKitConfig { get; set; }
    public SendGridConfig SendGridConfig { get; set; }
    public string From { get; set; }
    public string DisplayName { get; set; }
    public bool Enable { get; set; }
}

public class MimeKitConfig
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class SendGridConfig
{
    public string ApiKey { get; set; }
}
