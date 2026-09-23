namespace Ordering.Domain.Orders;

/// <summary>
/// Defines the domain factory used to create Order aggregate roots.
/// </summary>
public interface IOrderFactory
{
    /// <summary>
    /// Creates a new pending order for the supplied customer.
    /// </summary>
    /// <param name="customerId">The identity of the customer who owns the order.</param>
    /// <returns>A valid Order aggregate root ready to be persisted.</returns>
    Order Create(Guid customerId);

    /// <summary>
    /// Creates a new pending order and adds the first product line.
    /// </summary>
    /// <param name="customerId">The identity of the customer who owns the order.</param>
    /// <param name="productId">The identity of the product being ordered.</param>
    /// <param name="productName">The product name captured at ordering time.</param>
    /// <param name="unitPrice">The product unit price captured at ordering time.</param>
    /// <param name="quantity">The quantity requested by the customer.</param>
    /// <returns>A valid Order aggregate root with one order item.</returns>
    Order CreateWithItem(
        Guid customerId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity);
}
