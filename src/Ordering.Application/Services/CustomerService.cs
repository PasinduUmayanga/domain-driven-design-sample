using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Customers;
using Ordering.Domain.Customers;

namespace Ordering.Application.Services;

public sealed class CustomerService(ICustomerRepository customerRepository)
{
    public async Task<CustomerResponse> RegisterAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = Customer.Register(request.Name, request.Email);

        await customerRepository.AddAsync(customer, cancellationToken);

        return CustomerResponse.FromCustomer(customer);
    }

    public async Task<CustomerResponse> GetAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerAsync(customerId, cancellationToken);

        return CustomerResponse.FromCustomer(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = await GetCustomerAsync(customerId, cancellationToken);

        customer.UpdateProfile(request.Name, request.Email);

        await customerRepository.SaveAsync(customer, cancellationToken);

        return CustomerResponse.FromCustomer(customer);
    }

    public async Task<CustomerResponse> DeactivateAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerAsync(customerId, cancellationToken);

        customer.Deactivate();

        await customerRepository.SaveAsync(customer, cancellationToken);

        return CustomerResponse.FromCustomer(customer);
    }

    private async Task<Customer> GetCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));
        }

        var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken);

        return customer
            ?? throw new KeyNotFoundException("Customer was not found.");
    }
}
