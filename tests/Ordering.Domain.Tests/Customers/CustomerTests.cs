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

    [Fact]
    public void UpdateProfile_Should_Modify_Name_And_Email()
    {
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");

        customer.UpdateProfile("Grace Hopper", "grace@example.com");

        Assert.Equal("Grace Hopper", customer.Name);
        Assert.Equal("grace@example.com", customer.Email);
    }

    [Fact]
    public void UpdateProfile_Should_Not_Partially_Modify_When_Email_Is_Invalid()
    {
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");

        var action = () => customer.UpdateProfile("Grace Hopper", "not-an-email");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
    }
}
