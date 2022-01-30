namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

public record CreateCustomerRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password);
