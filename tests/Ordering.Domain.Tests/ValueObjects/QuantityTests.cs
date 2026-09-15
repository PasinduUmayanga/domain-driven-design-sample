using Ordering.Domain.Common.ValueObjects;

namespace Ordering.Domain.Tests.ValueObjects;

public class QuantityTests
{
    [Fact]
    public void Create_Should_Create_Positive_Quantity()
    {
        var quantity = Quantity.Create(2);

        Assert.Equal(2, quantity.Value);
    }

    [Fact]
    public void Create_Should_Throw_When_Quantity_Is_Not_Positive()
    {
        void action() => Quantity.Create(0);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
