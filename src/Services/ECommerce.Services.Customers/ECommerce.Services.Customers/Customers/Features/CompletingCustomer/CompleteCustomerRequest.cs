namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer;

public record CompleteCustomerRequest(
    string PhoneNumber,
    DateTime? BirthDate = null,
    string? Country = null,
    string? City = null,
    string? DetailAddress = null,
    string? Nationality = null);
