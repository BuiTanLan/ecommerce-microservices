using AutoMapper;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.CQRS.Query;
using BuildingBlocks.CQRS;
using BuildingBlocks.CQRS.Query;
using BuildingBlocks.Persistence.Mongo;
using ECommerce.Services.Customers.Customers.Dtos;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.Models.Reads;
using ECommerce.Services.Customers.Shared.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ECommerce.Services.Customers.Customers.Features.GettingCustomers;

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
    private readonly CustomersReadDbContext _customersReadDbContext;
    private readonly IMapper _mapper;

    public GetCustomersHandler(CustomersReadDbContext customersReadDbContext, IMapper mapper)
    {
        _customersReadDbContext = customersReadDbContext;
        _mapper = mapper;
    }

    public async Task<GetCustomersResult> Handle(GetCustomers request, CancellationToken cancellationToken)
    {
        var customer = await _customersReadDbContext.Customers.AsQueryable()
            .Where(x => request.CustomerState == CustomerState.None || x.CustomerState == request.CustomerState)
            .OrderByDescending(x => x.City)
            .ApplyFilterList(request.Filters)
            .PaginateAsync<CustomerReadModel, CustomerReadDto>(
                _mapper.ConfigurationProvider,
                request.Page,
                request.PageSize,
                cancellationToken: cancellationToken);

        return new GetCustomersResult(customer);
    }
}

public record GetCustomersResult(ListResultModel<CustomerReadDto> Customers);
