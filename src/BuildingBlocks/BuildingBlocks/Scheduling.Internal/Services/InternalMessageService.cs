using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Serialization;
using Humanizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Scheduling.Internal.Services;

public class InternalMessageService : IInternalMessageService
{
    private readonly InternalMessageSchedulerOptions _options;
    private readonly IMediator _mediator;
    private readonly InternalMessageDbContext _internalMessageDbContext;
    private readonly ILogger<InternalMessageService> _logger;
    private readonly IMessageSerializer _messageSerializer;

    public InternalMessageService(
        IOptions<InternalMessageSchedulerOptions> options,
        IMediator mediator,
        InternalMessageDbContext internalMessageDbContext,
        ILogger<InternalMessageService> logger,
        IMessageSerializer messageSerializer
    )
    {
        _options = options.Value;
        _mediator = mediator;
        _internalMessageDbContext = internalMessageDbContext;
        _logger = logger;
        _messageSerializer = messageSerializer;
    }

    public async Task<IEnumerable<InternalMessage>> GetAllUnsentInternalMessagesAsync(
        CancellationToken cancellationToken = default)
    {
        var messages = await _internalMessageDbContext.InternalMessages
            .Where(x => x.ProcessedOn == null)
            .ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    public async Task<IEnumerable<InternalMessage>> GetAllInternalMessagesAsync(CancellationToken cancellationToken =
        default)
    {
        var messages =
            await _internalMessageDbContext.InternalMessages.ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    public async Task SaveAsync(IInternalCommand internalCommand, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(internalCommand, nameof(internalCommand));

        if (!_options.Enabled)
        {
            _logger.LogWarning("Internal-Message is disabled, messages won't be saved");
            return;
        }

        string name = internalCommand.GetType().Name;

        var internalMessage = new InternalMessage(
            internalCommand.Id,
            internalCommand.OccurredOn,
            internalCommand.CommandType,
            name.Underscore(),
            _messageSerializer.Serialize(internalCommand),
            correlationId: null);

        await _internalMessageDbContext.InternalMessages.AddAsync(internalMessage, cancellationToken);
        await _internalMessageDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved message to the internal-messages store");
    }

    public async Task PublishUnsentInternalMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = await _internalMessageDbContext.InternalMessages
            .Where(x => x.ProcessedOn == null).ToListAsync(cancellationToken: cancellationToken);

        if (!unsentMessages.Any())
        {
            _logger.LogInformation("No unsent messages found in internal-messages store");
            return;
        }

        _logger.LogInformation(
            "Found {Count} unsent messages in internal-messages store, sending...",
            unsentMessages.Count);

        foreach (var internalMessage in unsentMessages)
        {
            var type = Type.GetType(internalMessage.Type);

            dynamic data = _messageSerializer.Deserialize(internalMessage.Data, type);
            if (data is null)
            {
                _logger.LogError("Invalid message type: {Name}", type?.Name);
                continue;
            }

            var internalCommand = data as IInternalCommand;

            await _mediator.Send(internalCommand, cancellationToken);

            _logger.LogInformation(
                "Sent a internal command: '{Name}' with ID: '{Id} (internal-message store)'",
                internalMessage.Name,
                internalCommand.Id);

            internalMessage.MarkAsProcessed();
        }

        await _internalMessageDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
