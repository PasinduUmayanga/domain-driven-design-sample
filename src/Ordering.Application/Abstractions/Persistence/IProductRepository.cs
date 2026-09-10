using Ordering.Domain.Products;

namespace Ordering.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task SaveAsync(Product product, CancellationToken cancellationToken = default);
}
