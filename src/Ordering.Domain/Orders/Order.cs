namespace Ordering.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    /// <summary>
    /// Gets the identity assigned when the order is created.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the customer that owns this order.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the current point in the order lifecycle.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp at which the order was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the items currently included in this order.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Gets the aggregate total of all order line items.
    /// </summary>
    public decimal TotalAmount =>
        _items.Sum(item => item.TotalPrice);

    // Reserved for ORM/materialization scenarios. Domain code must use Create so
    // every new order begins in a valid pending state.
    private Order()
    {
    }

    private Order(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new pending order for the supplied customer.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="customerId"/> is empty.</exception>
    public static Order Create(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));
        }

        return new Order(
            Guid.NewGuid(),
            customerId);
    }

    /// <summary>
    /// Adds a product to this pending order, or increases the quantity when that product is already present.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown unless the order is pending.</exception>
    /// <exception cref="ArgumentException">Thrown when the product identity or name is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the price or quantity is invalid.</exception>
    public void AddItem(
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        EnsurePending();

        var existingItem = _items.FirstOrDefault(
            item => item.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        var orderItem = new OrderItem(
            productId,
            productName,
            unitPrice,
            quantity);

        _items.Add(orderItem);
    }

    /// <summary>
    /// Removes a product line from this pending order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the order is not pending or the product is absent.</exception>
    public void RemoveItem(Guid productId)
    {
        EnsurePending();

        var item = _items.FirstOrDefault(
            item => item.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Product does not exist in the order.");
        }

        _items.Remove(item);
    }

    /// <summary>
    /// Replaces the quantity of an item in this pending order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the order is not pending or the product is absent.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="quantity"/> is not positive.</exception>
    public void ChangeItemQuantity(
        Guid productId,
        int quantity)
    {
        EnsurePending();

        var item = _items.FirstOrDefault(
            item => item.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Product does not exist in the order.");
        }

        item.ChangeQuantity(quantity);
    }

    /// <summary>
    /// Confirms a non-empty pending order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the order is not pending or has no items.</exception>
    public void Confirm()
    {
        EnsurePending();

        if (_items.Count == 0)
        {
            throw new InvalidOperationException(
                "An empty order cannot be confirmed.");
        }

        Status = OrderStatus.Confirmed;
    }

    /// <summary>
    /// Moves a pending order to the cancelled state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown unless the order is pending.</exception>
    public void Cancel()
    {
        EnsurePending();

        Status = OrderStatus.Cancelled;
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be modified.");
        }
    }
}
