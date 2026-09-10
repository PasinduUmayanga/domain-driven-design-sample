using Ordering.Domain.Products;

namespace Ordering.Application.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal UnitPrice,
    bool IsAvailable,
    DateTime CreatedAtUtc)
{
    internal static ProductResponse FromProduct(Product product) =>
        new(
            product.Id,
            product.Name,
            product.UnitPrice,
            product.IsAvailable,
            product.CreatedAtUtc);
}
