using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Messaging.Scheduling;
using BuildingBlocks.Messaging.Scheduling.Helpers;
using BuildingBlocks.Messaging.Serialization;
using BuildingBlocks.Scheduling.Internal.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Scheduling.Internal.MessagesScheduler;

public class InternalMessageScheduler : IMessagesScheduler
{
    private readonly InternalMessageSchedulerOptions _options;
    private readonly ILogger<InternalMessageScheduler> _logger;
    private readonly IInternalMessageService _internalMessageService;
    private readonly IMessageSerializer _messageSerializer;

    public InternalMessageScheduler(
        IOptions<InternalMessageSchedulerOptions> options,
        ILogger<InternalMessageScheduler> logger,
        IInternalMessageService internalMessageService,
        IMessageSerializer messageSerializer
    )
    {
        _options = options.Value;
        _logger = logger;
        _internalMessageService = internalMessageService;
        _messageSerializer = messageSerializer;
    }

    public Task EnqueueAsync<T>(T command, string description = null)
        where T : IInternalCommand
    {
        Guard.Against.Null(command, nameof(command));

        return _internalMessageService.SaveAsync(command);
    }

    public Task EnqueueAsync(MessageSerializedObject messageSerializedObject, string description = null)
    {
        dynamic req = _messageSerializer.Deserialize(
            messageSerializedObject.Data,
            messageSerializedObject.GetPayloadType());
        var command = req as IInternalCommand;
        return EnqueueAsync(command, description);
    }

    public Task ScheduleAsync<T>(T command, DateTimeOffset scheduleAt, string description = null)
        where T : IInternalCommand
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(
        MessageSerializedObject messageSerializedObject,
        DateTimeOffset scheduleAt,
        string description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync<T>(T command, TimeSpan delay, string description = null)
        where T : IInternalCommand
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(MessageSerializedObject messageSerializedObject, TimeSpan delay,
        string description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleRecurringAsync<T>(T command, string name, string cronExpression, string description = null)
        where T : IInternalCommand
    {
        throw new NotImplementedException();
    }

    public Task ScheduleRecurringAsync(MessageSerializedObject messageSerializedObject, string name,
        string cronExpression,
        string description = null)
    {
        throw new NotImplementedException();
    }
}
