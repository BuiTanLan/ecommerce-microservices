using ECommerce.Services.Customers.Customers.Clients.Dtos;

namespace ECommerce.Services.Customers.Customers.Clients;

public interface IIdentityApiClient
{
    Task<GetUserByEmailResponse?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<CreateUserResponse?> CreateUserIdentityAsync(
        CreateUserRequest createUserRequest,
        CancellationToken cancellationToken = default);
}
