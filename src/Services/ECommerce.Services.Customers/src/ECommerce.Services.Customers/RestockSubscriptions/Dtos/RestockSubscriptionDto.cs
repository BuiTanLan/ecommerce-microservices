namespace ECommerce.Services.Customers.RestockSubscriptions.Dtos;

public record RestockSubscriptionDto(long Id, long CustomerId, string Email, long ProductId, string ProductName);
