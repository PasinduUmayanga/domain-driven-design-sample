using Ordering.Domain.Orders;

namespace Ordering.Domain.Tests.Orders;

public class OrderTests
{
    [Fact]
    public void Create_Should_Create_Pending_Order()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var order = Order.Create(customerId);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Confirm_Should_Change_Status_To_Confirmed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        order.AddItem(Guid.NewGuid(), "Product", 10m, 1);

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void AddItem_Should_Add_Item_To_Order()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(productId, "Laptop", 250_000m, 2);

        // Assert
        var item = Assert.Single(order.Items);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(500_000m, item.TotalPrice);
        Assert.Equal(500_000m, order.TotalAmount);
    }

    [Fact]
    public void AddItem_Should_Increase_Quantity_When_Product_Already_Exists()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Product", 10m, 1);

        // Act
        order.AddItem(productId, "Product", 10m, 2);

        // Assert
        var item = Assert.Single(order.Items);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(30m, order.TotalAmount);
    }

    [Fact]
    public void Confirm_Should_Throw_When_Order_Has_No_Items()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid());

        // Act
        var action = () => order.Confirm();

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void RemoveItem_Should_Remove_Existing_Item()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Laptop", 250_000m, 2);

        // Act
        order.RemoveItem(productId);

        // Assert
        Assert.Empty(order.Items);
        Assert.Equal(0m, order.TotalAmount);
    }

    [Fact]
    public void ChangeItemQuantity_Should_Replace_Item_Quantity()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Laptop", 250_000m, 2);

        // Act
        order.ChangeItemQuantity(productId, 3);

        // Assert
        var item = Assert.Single(order.Items);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(750_000m, order.TotalAmount);
    }

    [Fact]
    public void Cancel_Should_Change_Status_To_Cancelled()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Confirm_Should_Throw_When_Order_Is_Already_Confirmed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        order.AddItem(Guid.NewGuid(), "Product", 10m, 1);

        order.Confirm();

        // Act
        var action = () => order.Confirm();

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
}
