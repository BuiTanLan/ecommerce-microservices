using AutoMapper;
using BuildingBlocks.CQRS;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;
using ECommerce.Services.Customers.RestockSubscriptions.Models;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptions;

public record GetRestockSubscriptions : ListQuery<GetRestockSubscriptionsResult>;

internal class GetRestockSubscriptionsValidator : AbstractValidator<GetRestockSubscriptions>
{
    public GetRestockSubscriptionsValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetProductsHandler : IQueryHandler<GetRestockSubscriptions, GetRestockSubscriptionsResult>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly IMapper _mapper;

    public GetProductsHandler(CustomersDbContext customersDbContext, IMapper mapper)
    {
        _customersDbContext = customersDbContext;
        _mapper = mapper;
    }

    public async Task<GetRestockSubscriptionsResult> Handle(GetRestockSubscriptions request, CancellationToken cancellationToken)
    {
        var restockSubscriptions = await _customersDbContext.RestockSubscriptions
            .OrderByDescending(x => x.Created)
            .ApplyIncludeList(request.Includes)
            .ApplyFilterList(request.Filters)
            .AsNoTracking()
            .PaginateAsync<RestockSubscription, RestockSubscriptionDto>(_mapper.ConfigurationProvider, request.Page, request.PageSize, cancellationToken: cancellationToken);

        return new GetRestockSubscriptionsResult(restockSubscriptions);
    }
}

public record GetRestockSubscriptionsResult(ListResultModel<RestockSubscriptionDto> RestockSubscriptions);
