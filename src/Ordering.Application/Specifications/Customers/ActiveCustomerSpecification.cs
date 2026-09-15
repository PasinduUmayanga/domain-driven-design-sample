using Ordering.Domain.Customers;

namespace Ordering.Application.Specifications.Customers;

/// <summary>
/// Checks whether a customer is allowed to place an order.
/// </summary>
public sealed class ActiveCustomerSpecification : ISpecification<Customer>
{
    /// <inheritdoc />
    public bool IsSatisfiedBy(Customer candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return candidate.IsActive;
    }
}
