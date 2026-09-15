using Ordering.Application.Abstractions.DomainEvents;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Orders;
using Ordering.Domain.Orders;

namespace Ordering.Application.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IProductRepository productRepository,
    IDomainEventDispatcher domainEventDispatcher)
{
    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new KeyNotFoundException("Customer was not found.");
        }

        if (!customer.IsActive)
        {
            throw new InvalidOperationException("Inactive customers cannot place orders.");
        }

        var order = Order.Create(customer.Id);

        await orderRepository.AddAsync(order, cancellationToken);
        await DispatchDomainEventsAsync(order, cancellationToken);

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

        var product = await productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException("Product was not found.");
        }

        if (!product.IsAvailable)
        {
            throw new InvalidOperationException("Unavailable products cannot be added to orders.");
        }

        order.AddItem(
            product.Id,
            product.Name,
            product.UnitPrice,
            request.Quantity);

        await orderRepository.SaveAsync(order, cancellationToken);
        await DispatchDomainEventsAsync(order, cancellationToken);

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
        await DispatchDomainEventsAsync(order, cancellationToken);

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
        await DispatchDomainEventsAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> ConfirmAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        order.Confirm();

        await orderRepository.SaveAsync(order, cancellationToken);
        await DispatchDomainEventsAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    public async Task<OrderResponse> CancelAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        order.Cancel();

        await orderRepository.SaveAsync(order, cancellationToken);
        await DispatchDomainEventsAsync(order, cancellationToken);

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

    private async Task DispatchDomainEventsAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        if (order.DomainEvents.Count == 0)
        {
            return;
        }

        await domainEventDispatcher.DispatchAsync(
            order.DomainEvents.ToArray(),
            cancellationToken);

        order.ClearDomainEvents();
    }
}
