using Ordering.Domain.Customers;

namespace Ordering.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task SaveAsync(Customer customer, CancellationToken cancellationToken = default);
}
