namespace ECommerce.Services.Customers.RestockSubscriptions.Models.Read;

public record RestockSubscriptionReadModel(
    long CustomerId,
    string CustomerName,
    long ProductId,
    string ProductName,
    bool Processed,
    DateTime? ProcessedTime = null);
