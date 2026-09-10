using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Orders;
using Ordering.Application.Services;
using Ordering.Domain.Orders;

namespace Ordering.Application.Tests.Orders;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Pending_Order()
    {
        var service = CreateService();
        var customerId = Guid.NewGuid();

        var order = await service.CreateAsync(new CreateOrderRequest(customerId));

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Empty(order.Items);
    }

    [Fact]
    public async Task AddItemAsync_Should_Add_Item_And_Update_Total()
    {
        var service = CreateService();
        var order = await service.CreateAsync(new CreateOrderRequest(Guid.NewGuid()));
        var productId = Guid.NewGuid();

        var updatedOrder = await service.AddItemAsync(
            order.Id,
            new AddOrderItemRequest(productId, "Laptop", 250_000m, 2));

        var item = Assert.Single(updatedOrder.Items);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(500_000m, item.TotalPrice);
        Assert.Equal(500_000m, updatedOrder.TotalAmount);
    }

    [Fact]
    public async Task ConfirmAsync_Should_Confirm_NonEmpty_Order()
    {
        var service = CreateService();
        var order = await service.CreateAsync(new CreateOrderRequest(Guid.NewGuid()));

        await service.AddItemAsync(
            order.Id,
            new AddOrderItemRequest(Guid.NewGuid(), "Keyboard", 20_000m, 1));

        var confirmedOrder = await service.ConfirmAsync(order.Id);

        Assert.Equal(OrderStatus.Confirmed, confirmedOrder.Status);
    }

    [Fact]
    public async Task ConfirmAsync_Should_Throw_When_Order_Is_Empty()
    {
        var service = CreateService();
        var order = await service.CreateAsync(new CreateOrderRequest(Guid.NewGuid()));

        var action = () => service.ConfirmAsync(order.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task GetAsync_Should_Throw_When_Order_Does_Not_Exist()
    {
        var service = CreateService();

        var action = () => service.GetAsync(Guid.NewGuid());

        await Assert.ThrowsAsync<KeyNotFoundException>(action);
    }

    private static OrderService CreateService() =>
        new(new FakeOrderRepository());

    private sealed class FakeOrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _orders = [];

        public Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            _orders.Add(order.Id, order);

            return Task.CompletedTask;
        }

        public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            _orders.TryGetValue(orderId, out var order);

            return Task.FromResult(order);
        }

        public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
        {
            _orders[order.Id] = order;

            return Task.CompletedTask;
        }
    }
}
