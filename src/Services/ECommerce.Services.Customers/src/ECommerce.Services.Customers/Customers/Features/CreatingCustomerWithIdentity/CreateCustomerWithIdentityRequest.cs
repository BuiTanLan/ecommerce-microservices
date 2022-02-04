namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomerWithIdentity;

public record CreateCustomerWithIdentityRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password);
