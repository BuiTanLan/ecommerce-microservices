using System.Data;
using System.Diagnostics;
using System.Text.Json;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events.Internal;
using MediatR;
using Microsoft.Extensions.Logging;


namespace BuildingBlocks.EFCore;

[DebuggerStepThrough]
public class TxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<TxBehavior<TRequest, TResponse>> _logger;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IDbContext _dbContext;

    public TxBehavior(
        IDbContext dbContext,
        ILogger<TxBehavior<TRequest, TResponse>> logger,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
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

            await _domainEventDispatcher.DispatchAsync(cancellationToken);

            await _dbContext.CommitTransactionAsync(cancellationToken);

            return response;
        });
    }
}
