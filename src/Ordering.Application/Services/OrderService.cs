using Ordering.Application.Abstractions.DomainEvents;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Orders;
using Ordering.Application.Specifications;
using Ordering.Domain.Customers;
using Ordering.Domain.Orders;
using Ordering.Domain.Products;

namespace Ordering.Application.Services;

/// <summary>
/// Coordinates ordering use cases by loading aggregate roots, invoking domain behavior,
/// saving changes, and dispatching domain events.
/// </summary>
public sealed class OrderService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IProductRepository productRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ISpecification<Customer> activeCustomerSpecification,
    ISpecification<Product> availableProductSpecification)
{
    /// <summary>
    /// Creates a pending order for an active customer.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when the customer does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the customer is inactive.</exception>
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

        // The specification gives this cross-aggregate eligibility rule a name
        // instead of burying the rule as a raw boolean check in the use case.
        if (!activeCustomerSpecification.IsSatisfiedBy(customer))
        {
            throw new InvalidOperationException("Inactive customers cannot place orders.");
        }

        var order = Order.Create(customer.Id);

        await orderRepository.AddAsync(order, cancellationToken);
        await DispatchDomainEventsAsync(order, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    /// <summary>
    /// Gets an order by identity.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="orderId"/> is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the order does not exist.</exception>
    public async Task<OrderResponse> GetAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);

        return OrderResponse.FromOrder(order);
    }

    /// <summary>
    /// Adds an available product to a pending order.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when the order or product does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the product is unavailable or the order cannot be modified.</exception>
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

        // The order aggregate owns order-item rules, while this specification
        // handles the product eligibility rule before the product is added.
        if (!availableProductSpecification.IsSatisfiedBy(product))
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

    /// <summary>
    /// Changes the quantity of an existing product line in a pending order.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the item does not exist or the order cannot be modified.</exception>
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

    /// <summary>
    /// Removes a product line from a pending order.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the item does not exist or the order cannot be modified.</exception>
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

    /// <summary>
    /// Confirms a non-empty pending order.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the order is empty or is not pending.</exception>
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

    /// <summary>
    /// Cancels a pending order.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the order is not pending.</exception>
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

        // Domain events are dispatched only after persistence succeeds. This
        // prevents publishing events for changes that were not saved.
        await domainEventDispatcher.DispatchAsync(
            order.DomainEvents.ToArray(),
            cancellationToken);

        // Clear events so the same in-memory aggregate does not publish them
        // again on the next request.
        order.ClearDomainEvents();
    }
}
