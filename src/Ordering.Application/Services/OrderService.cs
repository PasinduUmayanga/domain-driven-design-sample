using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Orders;
using Ordering.Domain.Orders;

namespace Ordering.Application.Services;

public sealed class OrderService(IOrderRepository orderRepository)
{
    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = Order.Create(request.CustomerId);

        await orderRepository.AddAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> GetAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> AddItemAsync(
        Guid orderId,
        AddOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = await GetOrderAsync(orderId, cancellationToken);

        order.AddItem(
            request.ProductId,
            request.ProductName,
            request.UnitPrice,
            request.Quantity);

        await orderRepository.SaveAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> ChangeItemQuantityAsync(
        Guid orderId,
        Guid productId,
        ChangeOrderItemQuantityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = await GetOrderAsync(orderId, cancellationToken);

        order.ChangeItemQuantity(productId, request.Quantity);

        await orderRepository.SaveAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> RemoveItemAsync(
        Guid orderId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        order.RemoveItem(productId);

        await orderRepository.SaveAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> ConfirmAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        order.Confirm();

        await orderRepository.SaveAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> CancelAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        order.Cancel();

        await orderRepository.SaveAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    private async Task<Order> GetOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID cannot be empty.",
                nameof(orderId));
        }

        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

        return order
            ?? throw new KeyNotFoundException("Order was not found.");
    }
}
