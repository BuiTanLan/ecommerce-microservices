using System.Data;
using System.Diagnostics;
using System.Text.Json;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events.Internal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;

[DebuggerStepThrough]
public class TxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly IDbFacadeResolver _dbFacadeResolver;
    private readonly ILogger<TxBehavior<TRequest, TResponse>> _logger;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public TxBehavior(
        IDbFacadeResolver dbFacadeResolver,
        ILogger<TxBehavior<TRequest, TResponse>> logger,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _dbFacadeResolver = dbFacadeResolver;
        _logger = logger;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is not ITxRequest) return next();

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

        var strategy = _dbFacadeResolver.Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            // Achieving atomicity
            await using var transaction = await _dbFacadeResolver.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try

            {
                var response = await next();

                _logger.LogInformation(
                    "{Prefix} Executed the {MediatrRequest} request",
                    nameof(TxBehavior<TRequest, TResponse>),
                    typeof(TRequest).FullName);

                await _domainEventDispatcher.DispatchAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return response;
            }
            catch (System.Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
