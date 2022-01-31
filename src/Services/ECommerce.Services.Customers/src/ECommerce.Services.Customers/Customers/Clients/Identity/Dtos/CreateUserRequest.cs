namespace ECommerce.Services.Customers.Customers.Clients.Dtos;

public record CreateUserRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string ConfirmPassword);
