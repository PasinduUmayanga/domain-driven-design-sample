using Ordering.Domain.Products;

namespace Ordering.Domain.Tests.Products;

public class ProductTests
{
    [Fact]
    public void Create_Should_Create_Available_Product()
    {
        var product = Product.Create("Laptop", 250_000m);

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(250_000m, product.UnitPrice);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void Create_Should_Throw_When_Price_Is_Negative()
    {
        var action = () => Product.Create("Laptop", -1m);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void MarkUnavailable_Should_Throw_When_Product_Is_Already_Unavailable()
    {
        var product = Product.Create("Laptop", 250_000m);
        product.MarkUnavailable();

        var action = () => product.MarkUnavailable();

        Assert.Throws<InvalidOperationException>(action);
    }
}
