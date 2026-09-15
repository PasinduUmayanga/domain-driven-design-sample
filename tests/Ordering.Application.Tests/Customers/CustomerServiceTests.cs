using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Customers;
using Ordering.Application.Services;
using Ordering.Domain.Customers;

namespace Ordering.Application.Tests.Customers;

public class CustomerServiceTests
{
    [Fact]
    public async Task RegisterAsync_Should_Add_Active_Customer()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);

        var customer = await service.RegisterAsync(
            new CreateCustomerRequest("Ada Lovelace", "ada@example.com"));

        var savedCustomer = await repository.GetByIdAsync(customer.Id);
        Assert.NotNull(savedCustomer);
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
        Assert.True(customer.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_Existing_Customer()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        await repository.AddAsync(customer);

        var updatedCustomer = await service.UpdateAsync(
            customer.Id,
            new UpdateCustomerRequest("Grace Hopper", "grace@example.com"));

        Assert.Equal(customer.Id, updatedCustomer.Id);
        Assert.Equal("Grace Hopper", updatedCustomer.Name);
        Assert.Equal("grace@example.com", updatedCustomer.Email);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_Customer_Does_Not_Exist()
    {
        var service = new CustomerService(new FakeCustomerRepository());

        var action = () => service.UpdateAsync(
            Guid.NewGuid(),
            new UpdateCustomerRequest("Grace Hopper", "grace@example.com"));

        await Assert.ThrowsAsync<KeyNotFoundException>(action);
    }

    [Fact]
    public async Task UpdateAsync_Should_Not_Partially_Modify_Customer_When_Email_Is_Invalid()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerService(repository);
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        await repository.AddAsync(customer);

        var action = () => service.UpdateAsync(
            customer.Id,
            new UpdateCustomerRequest("Grace Hopper", "not-an-email"));

        await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly Dictionary<Guid, Customer> _customers = [];

        public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            _customers.Add(customer.Id, customer);

            return Task.CompletedTask;
        }

        public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            _customers.TryGetValue(customerId, out var customer);

            return Task.FromResult(customer);
        }

        public Task SaveAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            _customers[customer.Id] = customer;

            return Task.CompletedTask;
        }
    }
}
