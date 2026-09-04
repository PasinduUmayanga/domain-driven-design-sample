namespace Ordering.Domain.Orders
{
    /// <summary>
    /// Represents the valid lifecycle states of an <see cref="Order"/>.
    /// </summary>
    public enum OrderStatus
    {
        // Explicit values preserve persisted representations if storage is added later.
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3
    }
}
