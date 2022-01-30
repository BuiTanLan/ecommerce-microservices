using System.Net.Http.Json;
using Ardalis.GuardClauses;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.Customers.Clients.Dtos;
using Microsoft.Extensions.Options;

namespace ECommerce.Services.Customers.Customers.Clients;

public class IdentityApiClient : IIdentityApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IdentityApiClientOptions _options;

    public IdentityApiClient(HttpClient httpClient, IOptions<IdentityApiClientOptions> options)
    {
        _httpClient = Guard.Against.Null(httpClient, nameof(httpClient));
        _options = Guard.Against.Null(options.Value, nameof(options));

        _httpClient.BaseAddress = new Uri(_options.BaseApiAddress);
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public async Task<GetUserByEmailResponse?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.InvalidEmail(email);

        var userIdentity = await _httpClient.GetFromJsonAsync<UserIdentityDto>(
            $"{_options.UsersEndpoint}/{email}",
            cancellationToken);

        return new GetUserByEmailResponse(userIdentity);
    }

    public async Task<CreateUserResponse?> CreateUserIdentityAsync(
        CreateUserRequest createUserRequest,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(createUserRequest, nameof(createUserRequest));

        var response = await _httpClient.PostAsJsonAsync(
            _options.UsersEndpoint,
            createUserRequest,
            cancellationToken);

        // throws if not 200-299
        response.EnsureSuccessStatusCode();

        var createdUser =
            await response.Content.ReadFromJsonAsync<UserIdentityDto>(cancellationToken: cancellationToken);

        return new CreateUserResponse(createdUser);
    }
}
