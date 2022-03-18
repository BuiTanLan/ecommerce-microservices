using Ardalis.GuardClauses;
using AutoMapper;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.Models.Reads;
using ECommerce.Services.Customers.Shared.Data;
using MicroBootstrap.Abstractions.CQRS.Command;
using MongoDB.Driver;

namespace ECommerce.Services.Customers.Customers.Features.UpdatingMongoCustomerReadsModel;

public record UpdateMongoCustomerReadsModel : InternalCommand
{
    public Guid Id { get; init; }
    public long CustomerId { get; init; }
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

internal class UpdateMongoCustomerReadsModelHandler : ICommandHandler<UpdateMongoCustomerReadsModel>
{
    private readonly CustomersReadDbContext _customersReadDbContext;
    private readonly IMapper _mapper;

    public UpdateMongoCustomerReadsModelHandler(CustomersReadDbContext customersReadDbContext, IMapper mapper)
    {
        _customersReadDbContext = customersReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateMongoCustomerReadsModel command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var filterDefinition =
            Builders<CustomerReadModel>.Filter
                .Eq(x => x.CustomerId, command.CustomerId);

        var updateDefinition =
            Builders<CustomerReadModel>.Update
                .Set(x => x.Email, command.Email)
                .Set(x => x.Country, command.Country)
                .Set(x => x.City, command.City)
                .Set(x => x.DetailAddress, command.DetailAddress)
                .Set(x => x.IdentityId, command.IdentityId)
                .Set(x => x.CustomerId, command.CustomerId)
                .Set(x => x.CustomerState, command.CustomerState)
                .Set(x => x.Nationality, command.Nationality)
                .Set(x => x.FirstName, command.FirstName)
                .Set(x => x.LastName, command.LastName)
                .Set(x => x.FullName, command.FullName)
                .Set(x => x.Notes, command.Notes)
                .Set(x => x.PhoneNumber, command.PhoneNumber)
                .Set(x => x.BirthDate, command.BirthDate)
                .Set(x => x.CompletedAt, command.CompletedAt)
                .Set(x => x.VerifiedAt, command.VerifiedAt)
                .Set(x => x.IsActive, command.IsActive);

        await _customersReadDbContext.Customers.UpdateOneAsync(
            filterDefinition,
            updateDefinition,
            new UpdateOptions(),
            cancellationToken);

        return Unit.Value;
    }
}
