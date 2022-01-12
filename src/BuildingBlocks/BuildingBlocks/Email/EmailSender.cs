using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Email;

using System;
using System.Threading.Tasks;
using Configs;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailSender : IEmailSender
{
    private readonly SendGridConfig _sendGridConfig;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<SendGridConfig> sendGridConfig, ILogger<EmailSender> logger)
    {
        _logger = logger;
        _sendGridConfig = Guard.Against.Null(sendGridConfig?.Value, nameof(SendGridConfig));
    }

    private SendGridClient SendGridClient => new(_sendGridConfig.ApiKey);

    public async Task SendAsync(EmailObject emailObject)
    {
        try
        {
            if (emailObject == null)
            {
                throw new ArgumentNullException(nameof(emailObject));
            }

            SendGridMessage message = new SendGridMessage()
            {
                Subject = emailObject.Subject,
                HtmlContent = emailObject.MailBody,
            };

            message.AddTo(new EmailAddress(emailObject.ReceiverEmail, emailObject.ReceiverName));

            if (!string.IsNullOrWhiteSpace(emailObject.SenderEmail))
            {
                message.From = new EmailAddress(emailObject.SenderEmail, emailObject.SenderName);
                message.ReplyTo = new EmailAddress(emailObject.SenderEmail, emailObject.SenderName);
            }

            Response response = await SendGridClient.SendEmailAsync(message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error sending email");
        }
    }
}
