namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer;

public record CompleteCustomerRequest(
    long CustomerId,
    string PhoneNumber,
    DateTime? BirthDate = null,
    string? Address = null,
    string? Nationality = null);
