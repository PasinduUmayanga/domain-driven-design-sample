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

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
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

        order.Confirm();

        // Act
        var action = () => order.Confirm();

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
}