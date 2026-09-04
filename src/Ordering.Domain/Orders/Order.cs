namespace Ordering.Domain.Orders;

public sealed class Order
{
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
    /// Moves a pending order to the confirmed state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown unless the order is pending.</exception>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be confirmed.");
        }

        Status = OrderStatus.Confirmed;
    }

    /// <summary>
    /// Moves a pending order to the cancelled state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown unless the order is pending.</exception>
    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}
