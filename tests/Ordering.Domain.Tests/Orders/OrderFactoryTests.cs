using Ordering.Domain.Orders;
using Ordering.Domain.Orders.Events;

namespace Ordering.Domain.Tests.Orders;

public class OrderFactoryTests
{
    [Fact]
    public void Create_Should_Create_Pending_Order_For_Customer()
    {
        // Arrange
        var factory = new OrderFactory();
        var customerId = Guid.NewGuid();

        // Act
        var order = factory.Create(customerId);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.IsType<OrderCreatedDomainEvent>(Assert.Single(order.DomainEvents));
    }

    [Fact]
    public void Create_Should_Throw_When_CustomerId_Is_Empty()
    {
        // Arrange
        var factory = new OrderFactory();

        // Act
        var action = () => factory.Create(Guid.Empty);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void CreateWithItem_Should_Create_Pending_Order_With_First_Item()
    {
        // Arrange
        var factory = new OrderFactory();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        var order = factory.CreateWithItem(
            customerId,
            productId,
            "Mechanical Keyboard",
            20_000m,
            2);

        // Assert
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(40_000m, order.TotalAmount);

        var item = Assert.Single(order.Items);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal("Mechanical Keyboard", item.ProductName);
        Assert.Equal(2, item.Quantity);

        Assert.Contains(
            order.DomainEvents,
            domainEvent => domainEvent is OrderCreatedDomainEvent);
        Assert.Contains(
            order.DomainEvents,
            domainEvent => domainEvent is OrderItemAddedDomainEvent);
    }

    [Fact]
    public void CreateWithItem_Should_Throw_When_First_Item_Is_Invalid()
    {
        // Arrange
        var factory = new OrderFactory();

        // Act
        var action = () => factory.CreateWithItem(
            Guid.NewGuid(),
            Guid.Empty,
            "Mechanical Keyboard",
            20_000m,
            1);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }
}
