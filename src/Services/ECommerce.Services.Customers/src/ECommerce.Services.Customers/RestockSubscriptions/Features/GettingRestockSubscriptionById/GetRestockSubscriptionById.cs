using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Query;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;
using ECommerce.Services.Customers.RestockSubscriptions.Exceptions.Application;
using ECommerce.Services.Customers.RestockSubscriptions.ValueObjects;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionById;

public record GetRestockSubscriptionById(long Id) : IQuery<GetRestockSubscriptionByIdResult>;

internal class GetRestockSubscriptionByIdValidator : AbstractValidator<GetRestockSubscriptionById>
{
    public GetRestockSubscriptionByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal class GetRestockSubscriptionByIdHandler
    : IQueryHandler<GetRestockSubscriptionById, GetRestockSubscriptionByIdResult>
{
    private readonly CustomersDbContext _customersDbContext;

    public GetRestockSubscriptionByIdHandler(CustomersDbContext customersDbContext)
    {
        _customersDbContext = customersDbContext;
    }

    public async Task<GetRestockSubscriptionByIdResult> Handle(
        GetRestockSubscriptionById query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var restockSubscription =
            await _customersDbContext.RestockSubscriptions.FindAsync(new RestockSubscriptionId(query.Id));
        Guard.Against.NotFound(restockSubscription, new RestockSubscriptionNotFound(query.Id));

        return new GetRestockSubscriptionByIdResult(new RestockSubscriptionDto(
            restockSubscription!.Id,
            restockSubscription.CustomerId,
            restockSubscription.Email!,
            restockSubscription.ProductInformation.Id,
            restockSubscription.ProductInformation.Name));
    }
}

public record GetRestockSubscriptionByIdResult(RestockSubscriptionDto RestockSubscription);
