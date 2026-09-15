using Ordering.Domain.Orders;
using Ordering.Domain.Orders.Events;

namespace Ordering.Domain.Tests.Orders;

public class OrderDomainEventTests
{
    [Fact]
    public void Create_Should_Record_OrderCreatedDomainEvent()
    {
        var customerId = Guid.NewGuid();

        var order = Order.Create(customerId);

        var domainEvent = Assert.Single(order.DomainEvents);
        var orderCreated = Assert.IsType<OrderCreatedDomainEvent>(domainEvent);
        Assert.Equal(order.Id, orderCreated.OrderId);
        Assert.Equal(customerId, orderCreated.CustomerId);
        Assert.NotEqual(default, orderCreated.OccurredAtUtc);
    }

    [Fact]
    public void AddItem_Should_Record_OrderItemAddedDomainEvent()
    {
        var order = Order.Create(Guid.NewGuid());
        order.ClearDomainEvents();
        var productId = Guid.NewGuid();

        order.AddItem(productId, "Keyboard", 20_000m, 2);

        var domainEvent = Assert.Single(order.DomainEvents);
        var itemAdded = Assert.IsType<OrderItemAddedDomainEvent>(domainEvent);
        Assert.Equal(order.Id, itemAdded.OrderId);
        Assert.Equal(productId, itemAdded.ProductId);
        Assert.Equal("Keyboard", itemAdded.ProductName);
        Assert.Equal(20_000m, itemAdded.UnitPrice);
        Assert.Equal(2, itemAdded.Quantity);
        Assert.NotEqual(default, itemAdded.OccurredAtUtc);
    }

    [Fact]
    public void Confirm_Should_Record_OrderConfirmedDomainEvent()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Keyboard", 20_000m, 2);
        order.ClearDomainEvents();

        order.Confirm();

        var domainEvent = Assert.Single(order.DomainEvents);
        var orderConfirmed = Assert.IsType<OrderConfirmedDomainEvent>(domainEvent);
        Assert.Equal(order.Id, orderConfirmed.OrderId);
        Assert.Equal(order.CustomerId, orderConfirmed.CustomerId);
        Assert.Equal(40_000m, orderConfirmed.TotalAmount);
        Assert.NotEqual(default, orderConfirmed.OccurredAtUtc);
    }

    [Fact]
    public void Cancel_Should_Record_OrderCancelledDomainEvent()
    {
        var order = Order.Create(Guid.NewGuid());
        order.ClearDomainEvents();

        order.Cancel();

        var domainEvent = Assert.Single(order.DomainEvents);
        var orderCancelled = Assert.IsType<OrderCancelledDomainEvent>(domainEvent);
        Assert.Equal(order.Id, orderCancelled.OrderId);
        Assert.Equal(order.CustomerId, orderCancelled.CustomerId);
        Assert.NotEqual(default, orderCancelled.OccurredAtUtc);
    }

    [Fact]
    public void Invalid_Operation_Should_Not_Record_DomainEvent()
    {
        var order = Order.Create(Guid.NewGuid());
        order.ClearDomainEvents();

        var action = () => order.Confirm();

        Assert.Throws<InvalidOperationException>(action);
        Assert.Empty(order.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_Should_Remove_Recorded_Events()
    {
        var order = Order.Create(Guid.NewGuid());

        order.ClearDomainEvents();

        Assert.Empty(order.DomainEvents);
    }
}
