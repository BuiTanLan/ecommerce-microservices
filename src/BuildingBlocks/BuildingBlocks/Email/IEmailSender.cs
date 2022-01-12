using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace BuildingBlocks.Email;

public interface IEmailSender
{
    Task SendAsync(EmailObject emailObject);
}

public class EmailObject
{
    public EmailObject(string receiverEmail, string subject, string mailBody)
    {
        ReceiverEmail = Guard.Against.NullOrEmpty(receiverEmail, nameof(receiverEmail));
        Subject = Guard.Against.NullOrEmpty(subject, nameof(subject));
        MailBody = Guard.Against.NullOrEmpty(mailBody, nameof(mailBody));
    }

    public EmailObject(string receiverEmail, string receiverName, string subject, string mailBody)
    {
        ReceiverName = Guard.Against.NullOrEmpty(receiverName, nameof(receiverName));
        ReceiverEmail = Guard.Against.NullOrEmpty(receiverEmail, nameof(receiverEmail));
        Subject = Guard.Against.NullOrEmpty(subject, nameof(subject));
        MailBody = Guard.Against.NullOrEmpty(mailBody, nameof(mailBody));
    }

    public EmailObject(
        string receiverEmail,
        string receiverName,
        string senderEmail,
        string senderName,
        string subject,
        string mailBody)
    {
        ReceiverEmail = Guard.Against.NullOrEmpty(receiverEmail, nameof(receiverEmail));
        ReceiverName = Guard.Against.NullOrEmpty(receiverName, nameof(receiverName));
        SenderEmail = Guard.Against.NullOrEmpty(senderEmail, nameof(senderEmail));
        SenderName = Guard.Against.NullOrEmpty(senderName, nameof(senderName));
        Subject = Guard.Against.NullOrEmpty(subject, nameof(subject));
        MailBody = Guard.Against.NullOrEmpty(mailBody, nameof(mailBody));
    }

    public string ReceiverEmail { get; }

    public string ReceiverName { get; }

    public string SenderEmail { get; }

    public string SenderName { get; }

    public string Subject { get; }

    public string MailBody { get; }
}
