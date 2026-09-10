namespace Ordering.Application.Orders;

public sealed record AddOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);
