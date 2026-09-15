using Money = Ordering.Domain.Common.ValueObjects.Money;
using ProductNameValue = Ordering.Domain.Common.ValueObjects.ProductName;
using QuantityValue = Ordering.Domain.Common.ValueObjects.Quantity;

namespace Ordering.Domain.Orders;

/// <summary>
/// Represents a product line within an order.
/// </summary>
public sealed class OrderItem : Ordering.Domain.Common.Entity
{
    private ProductNameValue _productName;

    private Money _unitPrice;

    private QuantityValue _quantity;

    /// <summary>
    /// Gets the identity of the product being ordered.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the product name captured when the item was added to the order.
    /// </summary>
    public string ProductName => _productName.Value;

    /// <summary>
    /// Gets the price per unit captured when the item was added to the order.
    /// </summary>
    public decimal UnitPrice => _unitPrice.Amount;

    /// <summary>
    /// Gets the number of units ordered.
    /// </summary>
    public int Quantity => _quantity.Value;

    /// <summary>
    /// Gets the line total derived from the captured unit price and quantity.
    /// </summary>
    public decimal TotalPrice => _unitPrice.Amount * _quantity.Value;

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

        var capturedProductName = ProductNameValue.Create(productName);
        var capturedUnitPrice = Money.Create(unitPrice, nameof(unitPrice));
        var itemQuantity = QuantityValue.Create(quantity);

        Id = Guid.NewGuid();
        ProductId = productId;
        _productName = capturedProductName;
        _unitPrice = capturedUnitPrice;
        _quantity = itemQuantity;
    }

    /// <summary>
    /// Increases the quantity on this line item.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="quantity"/> is not positive.</exception>
    internal void IncreaseQuantity(int quantity)
    {
        var additionalQuantity = QuantityValue.Create(quantity);

        _quantity = QuantityValue.Create(_quantity.Value + additionalQuantity.Value);
    }

    /// <summary>
    /// Replaces the quantity for this order line.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="quantity"/> is not positive.</exception>
    internal void ChangeQuantity(int quantity)
    {
        _quantity = QuantityValue.Create(quantity);
    }
}
