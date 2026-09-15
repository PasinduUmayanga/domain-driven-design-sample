using Ordering.Domain.Common.ValueObjects;

namespace Ordering.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_Should_Create_Money_For_NonNegative_Amount()
    {
        var money = Money.Create(250_000m);

        Assert.Equal(250_000m, money.Amount);
    }

    [Fact]
    public void Create_Should_Throw_When_Amount_Is_Negative()
    {
        void action() => Money.Create(-1m);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
