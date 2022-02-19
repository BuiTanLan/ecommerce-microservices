using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.Models.Reads;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Customers.Features.CreatingMongoCustomersReadModels;

public record CreateMongoCustomerReadModels : InternalCommand
{
    public long CustomerId { get; init; }
    public Guid Id { get; init; }
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
    public DateTime Created { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? VerifiedAt { get; init; }
    public CustomerState CustomerState { get; init; } = CustomerState.None;
}

internal class CreateMongoCustomerReadModelsHandler : ICommandHandler<CreateMongoCustomerReadModels>
{
    private readonly CustomersReadDbContext _customersReadDbContext;
    private readonly IMapper _mapper;

    public CreateMongoCustomerReadModelsHandler(CustomersReadDbContext customersReadDbContext, IMapper mapper)
    {
        _customersReadDbContext = customersReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateMongoCustomerReadModels command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var readModel = _mapper.Map<CustomerReadModel>(command);

        await _customersReadDbContext.Customers.InsertOneAsync(readModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
