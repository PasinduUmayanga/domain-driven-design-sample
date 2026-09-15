using Ordering.Domain.Common.ValueObjects;

namespace Ordering.Domain.Tests.ValueObjects;

public class ProductNameTests
{
    [Fact]
    public void Create_Should_Trim_Product_Name()
    {
        var productName = ProductName.Create("  Laptop  ");

        Assert.Equal("Laptop", productName.Value);
    }

    [Fact]
    public void Create_Should_Throw_When_Product_Name_Is_Empty()
    {
        void action() => ProductName.Create(" ");

        Assert.Throws<ArgumentException>(action);
    }
}
