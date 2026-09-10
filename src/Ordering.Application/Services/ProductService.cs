using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Products;
using Ordering.Domain.Products;

namespace Ordering.Application.Services;

public sealed class ProductService(IProductRepository productRepository)
{
    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = Product.Create(request.Name, request.UnitPrice);

        await productRepository.AddAsync(product, cancellationToken);

        return ProductResponse.FromProduct(product);
    }

    public async Task<ProductResponse> GetAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductAsync(productId, cancellationToken);

        return ProductResponse.FromProduct(product);
    }

    public async Task<ProductResponse> UpdateAsync(
        Guid productId,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = await GetProductAsync(productId, cancellationToken);

        product.Rename(request.Name);
        product.ChangePrice(request.UnitPrice);

        await productRepository.SaveAsync(product, cancellationToken);

        return ProductResponse.FromProduct(product);
    }

    public async Task<ProductResponse> MarkUnavailableAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductAsync(productId, cancellationToken);

        product.MarkUnavailable();

        await productRepository.SaveAsync(product, cancellationToken);

        return ProductResponse.FromProduct(product);
    }

    public async Task<ProductResponse> MarkAvailableAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductAsync(productId, cancellationToken);

        product.MarkAvailable();

        await productRepository.SaveAsync(product, cancellationToken);

        return ProductResponse.FromProduct(product);
    }

    private async Task<Product> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        return product
            ?? throw new KeyNotFoundException("Product was not found.");
    }
}
