using Ordering.Domain.Common;
using Ordering.Domain.Customers;
using Ordering.Domain.Orders;
using Ordering.Domain.Products;

namespace Ordering.Domain.Tests;

public class DomainModelArchitectureTests
{
    [Fact]
    public void Aggregate_roots_Should_Inherit_From_AggregateRoot()
    {
        Assert.IsAssignableFrom<AggregateRoot>(Order.Create(Guid.NewGuid()));
        Assert.IsAssignableFrom<AggregateRoot>(Customer.Register("Ada Lovelace", "ada@example.com"));
        Assert.IsAssignableFrom<AggregateRoot>(Product.Create("Keyboard", 20_000m));
    }

    [Fact]
    public void OrderItem_Should_Be_An_Entity_Inside_Order_Aggregate()
    {
        var order = Order.Create(Guid.NewGuid());

        order.AddItem(Guid.NewGuid(), "Keyboard", 20_000m, 1);

        var item = Assert.Single(order.Items);
        Assert.IsAssignableFrom<Entity>(item);
        Assert.False(
            typeof(OrderItem).GetConstructors().Any(),
            "Order items should be created through the Order aggregate root.");
    }
}
