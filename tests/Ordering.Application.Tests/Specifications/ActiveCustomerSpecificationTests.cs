using Ordering.Application.Specifications.Customers;
using Ordering.Domain.Customers;

namespace Ordering.Application.Tests.Specifications;

public class ActiveCustomerSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_Should_Return_True_For_Active_Customer()
    {
        var specification = new ActiveCustomerSpecification();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");

        Assert.True(specification.IsSatisfiedBy(customer));
    }

    [Fact]
    public void IsSatisfiedBy_Should_Return_False_For_Inactive_Customer()
    {
        var specification = new ActiveCustomerSpecification();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        customer.Deactivate();

        Assert.False(specification.IsSatisfiedBy(customer));
    }
}
