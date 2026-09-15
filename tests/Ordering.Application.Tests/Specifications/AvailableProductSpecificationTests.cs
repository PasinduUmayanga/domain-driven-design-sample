using Ordering.Application.Specifications.Products;
using Ordering.Domain.Products;

namespace Ordering.Application.Tests.Specifications;

public class AvailableProductSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_Should_Return_True_For_Available_Product()
    {
        var specification = new AvailableProductSpecification();
        var product = Product.Create("Laptop", 250_000m);

        Assert.True(specification.IsSatisfiedBy(product));
    }

    [Fact]
    public void IsSatisfiedBy_Should_Return_False_For_Unavailable_Product()
    {
        var specification = new AvailableProductSpecification();
        var product = Product.Create("Laptop", 250_000m);
        product.MarkUnavailable();

        Assert.False(specification.IsSatisfiedBy(product));
    }
}
