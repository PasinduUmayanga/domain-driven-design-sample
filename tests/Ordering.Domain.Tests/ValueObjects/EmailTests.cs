using Ordering.Domain.Common.ValueObjects;

namespace Ordering.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_Should_Trim_Email()
    {
        var email = Email.Create("  ada@example.com  ");

        Assert.Equal("ada@example.com", email.Value);
    }

    [Fact]
    public void Create_Should_Throw_When_Email_Is_Invalid()
    {
        void action() => Email.Create("not-an-email");

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    [InlineData("ada@@example.com")]
    [InlineData("ada example@example.com")]
    public void Create_Should_Throw_When_Email_Format_Is_Invalid(string value)
    {
        void action() => Email.Create(value);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Same_Email_Values_Should_Be_Equal()
    {
        var first = Email.Create("ada@example.com");
        var second = Email.Create("ada@example.com");

        Assert.Equal(first, second);
    }
}
