using System.Collections.Concurrent;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Customers;

namespace Ordering.Infrastructure.Customers;

public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _customers = new();

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (!_customers.TryAdd(customer.Id, customer))
        {
            throw new InvalidOperationException("Customer already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        _customers.TryGetValue(customerId, out var customer);

        return Task.FromResult(customer);
    }

    public Task SaveAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        _customers[customer.Id] = customer;

        return Task.CompletedTask;
    }
}
