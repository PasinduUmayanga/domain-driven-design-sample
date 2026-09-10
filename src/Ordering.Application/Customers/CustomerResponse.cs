using Ordering.Domain.Customers;

namespace Ordering.Application.Customers;

public sealed record CustomerResponse(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    DateTime CreatedAtUtc)
{
    internal static CustomerResponse FromCustomer(Customer customer) =>
        new(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.IsActive,
            customer.CreatedAtUtc);
}
