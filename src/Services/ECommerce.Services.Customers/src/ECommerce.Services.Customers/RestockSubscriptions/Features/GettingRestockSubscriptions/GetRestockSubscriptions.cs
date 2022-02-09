using AutoMapper;
using BuildingBlocks.CQRS;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;
using ECommerce.Services.Customers.RestockSubscriptions.Models;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Write;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptions;

public record GetRestockSubscriptions : ListQuery<GetRestockSubscriptionsResult>
{
    public IList<string>? Emails { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}

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

    public async Task<GetRestockSubscriptionsResult> Handle(
        GetRestockSubscriptions query,
        CancellationToken cancellationToken)
    {
        var restockSubscriptions = await _customersDbContext.RestockSubscriptions
            .OrderByDescending(x => x.Created)
            .ApplyIncludeList(query.Includes)
            .ApplyFilterList(query.Filters)
            .AsNoTracking()
            .Where(x => query.Emails == null || query.Emails.Any() == false || query.Emails.Contains(x.Email!))
            .Where(x => (query.From == null && query.To == null) || (query.From == null && x.Created <= query.To) ||
                        (query.To == null && x.Created >= query.From) ||
                        (x.Created >= query.From && x.Created <= query.To))
            .PaginateAsync<RestockSubscription, RestockSubscriptionDto>(
                _mapper.ConfigurationProvider,
                query.Page,
                query.PageSize,
                cancellationToken: cancellationToken);

        return new GetRestockSubscriptionsResult(restockSubscriptions);
    }
}

public record GetRestockSubscriptionsResult(ListResultModel<RestockSubscriptionDto> RestockSubscriptions);
