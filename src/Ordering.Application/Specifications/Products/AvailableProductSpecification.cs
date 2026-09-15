using Ordering.Domain.Products;

namespace Ordering.Application.Specifications.Products;

/// <summary>
/// Checks whether a product is available to be added to an order.
/// </summary>
public sealed class AvailableProductSpecification : ISpecification<Product>
{
    /// <inheritdoc />
    public bool IsSatisfiedBy(Product candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return candidate.IsAvailable;
    }
}
