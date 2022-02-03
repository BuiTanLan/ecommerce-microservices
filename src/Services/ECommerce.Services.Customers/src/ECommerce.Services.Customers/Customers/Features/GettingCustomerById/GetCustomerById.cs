using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Query;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.Customers.Dtos;
using ECommerce.Services.Customers.Customers.Exceptions.Application;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Customers.Features.GettingCustomerById;

public record GetCustomerById(long Id) : IQuery<GetCustomerByIdResult>;

public record GetCustomerByIdResult(CustomerDto Customer);

internal class GetCustomerByIdValidator : AbstractValidator<GetCustomerById>
{
    public GetCustomerByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal class GetRestockSubscriptionByIdHandler
    : IQueryHandler<GetCustomerById, GetCustomerByIdResult>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly IMapper _mapper;

    public GetRestockSubscriptionByIdHandler(CustomersDbContext customersDbContext, IMapper mapper)
    {
        _customersDbContext = customersDbContext;
        _mapper = mapper;
    }

    public async Task<GetCustomerByIdResult> Handle(
        GetCustomerById query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var customer = await _customersDbContext.Customers.FindAsync(query.Id);
        Guard.Against.NotFound(customer, new CustomerNotFoundException(query.Id));

        var customerDto = _mapper.Map<CustomerDto>(customer);

        return new GetCustomerByIdResult(customerDto);
    }
}
