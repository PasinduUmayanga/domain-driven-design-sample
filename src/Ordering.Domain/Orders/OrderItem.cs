namespace Ordering.Domain.Orders;

/// <summary>
/// Represents a product line within an order.
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// Gets the identity assigned when the line item is created.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identity of the product being ordered.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the product name captured when the item was added to the order.
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the price per unit captured when the item was added to the order.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the number of units ordered.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the line total derived from the captured unit price and quantity.
    /// </summary>
    public decimal TotalPrice => UnitPrice * Quantity;

    // Reserved for ORM/materialization scenarios. New items must be created through
    // the internal constructor so their product and quantity invariants are checked.
    private OrderItem()
    {
    }

    /// <summary>
    /// Creates an order line for a product.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the product identity or name is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the price or quantity is invalid.</exception>
    internal OrderItem(
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product ID cannot be empty.",
                nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException(
                "Product name cannot be empty.",
                nameof(productName));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Unit price cannot be negative.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    /// <summary>
    /// Increases the quantity on this line item.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="quantity"/> is not positive.</exception>
    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        Quantity += quantity;
    }
}
