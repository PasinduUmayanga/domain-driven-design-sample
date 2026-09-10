using Ordering.Application.Products;
using Ordering.Application.Services;

namespace Ordering.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var products = app.MapGroup("/products");

        products.MapPost(
                "/",
                async (
                    CreateProductRequest request,
                    ProductService productService,
                    CancellationToken cancellationToken) =>
                {
                    var product = await productService.CreateAsync(request, cancellationToken);

                    return Results.Created($"/products/{product.Id}", product);
                })
            .WithName("CreateProduct");

        products.MapGet(
                "/{productId:guid}",
                async (
                    Guid productId,
                    ProductService productService,
                    CancellationToken cancellationToken) =>
                {
                    var product = await productService.GetAsync(productId, cancellationToken);

                    return Results.Ok(product);
                })
            .WithName("GetProduct");

        products.MapPut(
                "/{productId:guid}",
                async (
                    Guid productId,
                    UpdateProductRequest request,
                    ProductService productService,
                    CancellationToken cancellationToken) =>
                {
                    var product = await productService.UpdateAsync(
                        productId,
                        request,
                        cancellationToken);

                    return Results.Ok(product);
                })
            .WithName("UpdateProduct");

        products.MapPost(
                "/{productId:guid}/mark-unavailable",
                async (
                    Guid productId,
                    ProductService productService,
                    CancellationToken cancellationToken) =>
                {
                    var product = await productService.MarkUnavailableAsync(
                        productId,
                        cancellationToken);

                    return Results.Ok(product);
                })
            .WithName("MarkProductUnavailable");

        products.MapPost(
                "/{productId:guid}/mark-available",
                async (
                    Guid productId,
                    ProductService productService,
                    CancellationToken cancellationToken) =>
                {
                    var product = await productService.MarkAvailableAsync(
                        productId,
                        cancellationToken);

                    return Results.Ok(product);
                })
            .WithName("MarkProductAvailable");

        return app;
    }
}
