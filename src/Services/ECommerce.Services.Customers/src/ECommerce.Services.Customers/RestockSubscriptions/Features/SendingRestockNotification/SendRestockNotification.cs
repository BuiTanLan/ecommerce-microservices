using Ardalis.GuardClauses;
using BuildingBlocks.Core.Persistence;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Email;
using BuildingBlocks.Email.Options;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.RestockSubscriptions.Exceptions.Domain;
using ECommerce.Services.Customers.RestockSubscriptions.Features.MarkingRestockSubscriptionAsProcessed;
using ECommerce.Services.Customers.Shared.Data;
using Microsoft.Extensions.Options;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.SendingRestockNotification;

public record SendRestockNotification(long ProductId, int CurrentStock) : InternalCommand, ITxRequest;

internal class SendRestockNotificationValidator : AbstractValidator<SendRestockNotification>
{
    public SendRestockNotificationValidator()
    {
        RuleFor(x => x.CurrentStock)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}

internal class SendRestockNotificationHandler : ICommandHandler<SendRestockNotification>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly IEmailSender _emailSender;
    private readonly ICommandProcessor _commandProcessor;
    private readonly EmailOptions _emailConfig;
    private readonly ILogger<SendRestockNotificationHandler> _logger;

    public SendRestockNotificationHandler(
        CustomersDbContext customersDbContext,
        IEmailSender emailSender,
        ICommandProcessor commandProcessor,
        IOptions<EmailOptions> emailConfig,
        ILogger<SendRestockNotificationHandler> logger)
    {
        _customersDbContext = customersDbContext;
        _emailSender = emailSender;
        _commandProcessor = commandProcessor;
        _emailConfig = emailConfig.Value;
        _logger = logger;
    }

    public async Task<Unit> Handle(SendRestockNotification command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, new RestockSubscriptionDomainException("Command cannot be null"));

        var subscribedCustomers =
            _customersDbContext.RestockSubscriptions.Where(x =>
                x.ProductInformation.Id == command.ProductId && x.Processed == false);

        foreach (var restockSubscription in subscribedCustomers)
        {
            if (_emailConfig.Enable)
            {
                await _emailSender.SendAsync(
                    new EmailObject(
                        restockSubscription.Email!,
                        _emailConfig.From,
                        "Restock Notification",
                        $"Your product {restockSubscription.ProductInformation.Name} is back in stock. Current stock is {command.CurrentStock}"));

                await _commandProcessor.SendAsync(
                    new MarkRestockSubscriptionAsProcessed(restockSubscription.Id),
                    cancellationToken);

                _logger.LogInformation("Restock notification sent to email {Email}", restockSubscription.Email);
            }
        }

        return Unit.Value;
    }
}
