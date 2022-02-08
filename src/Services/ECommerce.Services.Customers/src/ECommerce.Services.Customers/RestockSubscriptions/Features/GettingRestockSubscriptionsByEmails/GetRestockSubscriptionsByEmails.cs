using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.GettingRestockSubscriptionsByEmails;

public record GetRestockSubscriptionsByEmails(IList<string> Emails) : IStreamQuery<RestockSubscriptionDto>;

internal class GetRestockSubscriptionsByEmailsValidator : AbstractValidator<GetRestockSubscriptionsByEmails>
{
    public GetRestockSubscriptionsByEmailsValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Emails)
            .NotNull()
            .NotEmpty();
    }
}

internal class GetRestockSubscriptionsByEmailsHandler
    : IStreamQueryHandler<GetRestockSubscriptionsByEmails, RestockSubscriptionDto>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly IMapper _mapper;

    public GetRestockSubscriptionsByEmailsHandler(CustomersDbContext customersDbContext, IMapper mapper)
    {
        _customersDbContext = customersDbContext;
        _mapper = mapper;
    }

    public IAsyncEnumerable<RestockSubscriptionDto> Handle(
        GetRestockSubscriptionsByEmails query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var result = _customersDbContext.RestockSubscriptions
            .Where(x => query.Emails.Contains(x.Email!))
            .ProjectTo<RestockSubscriptionDto>(_mapper.ConfigurationProvider)
            .ToAsyncEnumerable();

        return result;
    }
}
