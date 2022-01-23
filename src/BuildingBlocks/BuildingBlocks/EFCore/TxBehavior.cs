using System.Data;
using System.Diagnostics;
using System.Text.Json;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Messaging.Outbox;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;

[DebuggerStepThrough]
public class TxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<TxBehavior<TRequest, TResponse>> _logger;
    private readonly IDomainEventContext _domainEventContext;
    private readonly IOutboxService _outboxService;
    private readonly IMediator _mediator;
    private readonly IDbContext _dbContext;

    public TxBehavior(
        IDbContext dbContext,
        ILogger<TxBehavior<TRequest, TResponse>> logger,
        IDomainEventContext domainEventContext,
        IOutboxService outboxService,
        IMediator mediator)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _domainEventContext = domainEventContext ?? throw new ArgumentNullException(nameof(domainEventContext));
        _outboxService = outboxService ?? throw new ArgumentNullException(nameof(outboxService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is not ITxRequest) return await next();

        _logger.LogInformation(
            "{Prefix} Handled command {MediatrRequest}",
            nameof(TxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        _logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
            nameof(TxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request));

        _logger.LogInformation(
            "{Prefix} Open the transaction for {MediatrRequest}",
            nameof(TxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        return await _dbContext.RetryOnExceptionAsync(async () =>
        {
            // Achieving atomicity
            await _dbContext.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var response = await next();

            _logger.LogInformation(
                "{Prefix} Executed the {MediatrRequest} request",
                nameof(TxBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName);

            // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
            // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
            // http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
            // http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
            // https://www.ledjonbehluli.com/posts/domain_to_integration_event/
            var events = _domainEventContext.GetDomainEvents()?.ToArray();
            await _mediator.DispatchDomainEventAsync(events, cancellationToken: cancellationToken);

            // save wrapped integration and notification events to outbox for further processing after commit
            await _outboxService.SaveAsync(events?.GetDomainNotificationEventsFromDomainEvents()?.ToArray());
            await _outboxService.SaveAsync(events?.GetIntegrationEventsFromDomainEvents()?.ToArray());

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return response;
        });
    }
}
