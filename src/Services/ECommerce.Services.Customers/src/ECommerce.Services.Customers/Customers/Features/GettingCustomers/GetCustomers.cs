using AutoMapper;
using BuildingBlocks.CQRS;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Customers.Customers.Dtos;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Catalogs.Customers.Features.GettingCustomers;

public record GetCustomers : ListQuery<GetCustomersResult>
{
    public CustomerState CustomerState { get; init; }
}

public class GetCustomersValidator : AbstractValidator<GetCustomers>
{
    public GetCustomersValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetCustomersHandler : IQueryHandler<GetCustomers, GetCustomersResult>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly IMapper _mapper;

    public GetCustomersHandler(CustomersDbContext customersDbContext, IMapper mapper)
    {
        _customersDbContext = customersDbContext;
        _mapper = mapper;
    }

    public async Task<GetCustomersResult> Handle(GetCustomers request, CancellationToken cancellationToken)
    {
        var customer = await _customersDbContext.Customers
            .Where(x => request.CustomerState == CustomerState.None || x.CustomerState == request.CustomerState)
            .OrderByDescending(x => x.Created)
            .ApplyIncludeList(request.Includes)
            .ApplyFilterList(request.Filters)
            .AsNoTracking()
            .PaginateAsync<Customer, CustomerDto>(_mapper.ConfigurationProvider, request.Page, request.PageSize, cancellationToken: cancellationToken);

        return new GetCustomersResult(customer);
    }
}

public record GetCustomersResult(ListResultModel<CustomerDto> Customers);
