using Ordering.Domain.Customers;

namespace Ordering.Domain.Tests.Customers;

public class CustomerTests
{
    [Fact]
    public void Register_Should_Create_Active_Customer()
    {
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
        Assert.True(customer.IsActive);
    }

    [Fact]
    public void Register_Should_Throw_When_Email_Is_Invalid()
    {
        var action = () => Customer.Register("Ada Lovelace", "not-an-email");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void ChangeName_Should_Throw_When_Customer_Is_Inactive()
    {
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        customer.Deactivate();

        var action = () => customer.ChangeName("Grace Hopper");

        Assert.Throws<InvalidOperationException>(action);
    }
}
