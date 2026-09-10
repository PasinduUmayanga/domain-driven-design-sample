using System.Collections.Concurrent;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Products;

namespace Ordering.Infrastructure.Products;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (!_products.TryAdd(product.Id, product))
        {
            throw new InvalidOperationException("Product already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        _products.TryGetValue(productId, out var product);

        return Task.FromResult(product);
    }

    public Task SaveAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);

        _products[product.Id] = product;

        return Task.CompletedTask;
    }
}
