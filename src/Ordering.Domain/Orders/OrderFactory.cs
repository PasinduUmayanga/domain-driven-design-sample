namespace Ordering.Domain.Orders;

/// <summary>
/// Centralizes Order aggregate creation when application code needs a new order.
/// </summary>
public sealed class OrderFactory : IOrderFactory
{
    /// <summary>
    /// Creates a new pending order through the aggregate's factory method.
    /// </summary>
    /// <param name="customerId">The identity of the customer who owns the order.</param>
    /// <returns>A valid Order aggregate root with its creation domain event recorded.</returns>
    public Order Create(Guid customerId)
    {
        // The aggregate still owns its invariants. The factory gives the
        // application layer one creation dependency that can grow as order
        // construction becomes more complex.
        return Order.Create(customerId);
    }

    /// <summary>
    /// Creates a pending order and adds the first item before returning it.
    /// </summary>
    /// <param name="customerId">The identity of the customer who owns the order.</param>
    /// <param name="productId">The identity of the product being ordered.</param>
    /// <param name="productName">The product name captured at ordering time.</param>
    /// <param name="unitPrice">The product unit price captured at ordering time.</param>
    /// <param name="quantity">The quantity requested by the customer.</param>
    /// <returns>A valid Order aggregate root with creation and item-added events recorded.</returns>
    public Order CreateWithItem(
        Guid customerId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        // This method demonstrates a factory coordinating multiple aggregate
        // operations while leaving all rule checks inside the aggregate itself.
        var order = Order.Create(customerId);

        order.AddItem(
            productId,
            productName,
            unitPrice,
            quantity);

        return order;
    }
}
