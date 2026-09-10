using System.Collections.Concurrent;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Orders;

namespace Ordering.Infrastructure.Orders;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException("Order already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(orderId, out var order);

        return Task.FromResult(order);
    }

    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        _orders[order.Id] = order;

        return Task.CompletedTask;
    }
}
