using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.ValueObjects;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Exception;
using BuildingBlocks.IdsGenerator;
using ECommerce.Services.Customers.Customers.Exceptions.Application;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;
using ECommerce.Services.Customers.RestockSubscriptions.Models;
using ECommerce.Services.Customers.RestockSubscriptions.ValueObjects;
using ECommerce.Services.Customers.RestockSubscriptions.ValueObjects.Exceptions;
using ECommerce.Services.Customers.Shared.Clients.Catalogs;
using ECommerce.Services.Customers.Shared.Data;
using ECommerce.Services.Customers.Shared.Extensions;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription;

public record CreateRestockSubscription(long CustomerId, long ProductId, string Email)
    : ICreateCommand<CreateRestockSubscriptionResult>
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
};

internal class CreateRestockSubscriptionValidator : AbstractValidator<CreateRestockSubscription>
{
    public CreateRestockSubscriptionValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

public record CreateRestockSubscriptionResult(RestockSubscriptionDto RestockSubscription);

internal class CreateRestockSubscriptionHandler
    : ICommandHandler<CreateRestockSubscription, CreateRestockSubscriptionResult>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly ICatalogApiClient _catalogApiClient;
    private readonly ILogger<CreateRestockSubscriptionHandler> _logger;

    public CreateRestockSubscriptionHandler(
        CustomersDbContext customersDbContext,
        ICatalogApiClient catalogApiClient,
        ILogger<CreateRestockSubscriptionHandler> logger)
    {
        _customersDbContext = customersDbContext;
        _catalogApiClient = catalogApiClient;
        _logger = logger;
    }

    public async Task<CreateRestockSubscriptionResult> Handle(
        CreateRestockSubscription request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var existsCustomer = await _customersDbContext.ExistsCustomerByIdAsync(request.CustomerId);
        Guard.Against.NotExists(existsCustomer, new CustomerNotFoundException(request.CustomerId));

        var product = (await _catalogApiClient.GetProductByIdAsync(request.ProductId, cancellationToken))?.Product;
        Guard.Against.NotFound(product, new ProductNotFoundException(request.ProductId));

        var restockSubscription =
            RestockSubscription.Create(
                request.Id,
                request.CustomerId,
                ProductInformation.Create(product!.Id, product.Name),
                Email.Create(request.Email));

        await _customersDbContext.AddAsync(restockSubscription, cancellationToken);

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RestockSubscription with id '{@Id}' saved successfully", restockSubscription.Id);

        return new CreateRestockSubscriptionResult(
            new RestockSubscriptionDto(
                restockSubscription.Id,
                restockSubscription.CustomerId,
                restockSubscription.Email!,
                restockSubscription.ProductInformation.Id,
                restockSubscription.ProductInformation.Name));
    }
}
