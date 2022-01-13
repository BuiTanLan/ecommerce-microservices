using BuildingBlocks.Email.Configs;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BuildingBlocks.Email;

public class MimeKitEmailSender : IEmailSender
{
    private readonly EmailConfig _config;
    private readonly ILogger<MimeKitEmailSender> _logger;

    public MimeKitEmailSender(IOptions<EmailConfig> config, ILogger<MimeKitEmailSender> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task SendAsync(EmailObject emailObject)
    {
        try
        {
            var email = new MimeMessage { Sender = MailboxAddress.Parse(emailObject.SenderEmail ?? _config.From) };
            email.To.Add(MailboxAddress.Parse(emailObject.ReceiverEmail));
            email.Subject = emailObject.Subject;
            var builder = new BodyBuilder { HtmlBody = emailObject.MailBody };
            email.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_config.MimeKitConfig.Host, _config.MimeKitConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config.MimeKitConfig.UserName, _config.MimeKitConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation(
                "Email sent. From: {From}, To: {To}, Subject: {Subject}, Content: {Content}",
                _config.From,
                emailObject.ReceiverEmail,
                emailObject.Subject,
                emailObject.MailBody);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }
}
