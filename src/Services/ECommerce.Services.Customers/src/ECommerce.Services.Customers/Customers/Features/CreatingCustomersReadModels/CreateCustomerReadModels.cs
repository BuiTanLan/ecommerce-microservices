using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.Models.Reads;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomersReadModels;

public record CreateCustomerReadModels : ITxCommand
{
    public long Id { get; init; }
    public Guid IdentityId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? DetailAddress { get; init; }
    public string? Nationality { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? VerifiedAt { get; init; }
    public CustomerState CustomerState { get; init; } = CustomerState.None;
}

internal class CreateCustomerReadModelsHandler : ICommandHandler<CreateCustomerReadModels>
{
    private readonly CustomersReadDbContext _customersReadDbContext;
    private readonly IMapper _mapper;

    public CreateCustomerReadModelsHandler(CustomersReadDbContext customersReadDbContext, IMapper mapper)
    {
        _customersReadDbContext = customersReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateCustomerReadModels command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var readModel = _mapper.Map<CustomerReadModel>(command);

        await _customersReadDbContext.Customers.InsertOneAsync(readModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
