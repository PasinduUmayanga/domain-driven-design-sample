namespace Ordering.Application.Orders;

public sealed record AddOrderItemRequest(
    Guid ProductId,
    int Quantity);
