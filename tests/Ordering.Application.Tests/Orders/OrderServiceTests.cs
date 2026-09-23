using Ordering.Application.Abstractions.DomainEvents;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Orders;
using Ordering.Application.Services;
using Ordering.Application.Specifications.Customers;
using Ordering.Application.Specifications.Products;
using Ordering.Domain.Common;
using Ordering.Domain.Customers;
using Ordering.Domain.Orders;
using Ordering.Domain.Orders.Events;
using Ordering.Domain.Products;

namespace Ordering.Application.Tests.Orders;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Pending_Order()
    {
        var service = CreateService();
        var context = CreateContext();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");

        await context.Customers.AddAsync(customer);
        service = CreateService(context);

        var order = await service.CreateAsync(new CreateOrderRequest(customer.Id));

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customer.Id, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Empty(order.Items);

        var savedOrder = await context.Orders.GetByIdAsync(order.Id);
        Assert.NotNull(savedOrder);
        Assert.Empty(savedOrder.DomainEvents);
        Assert.IsType<OrderCreatedDomainEvent>(
            Assert.Single(context.DomainEvents.DispatchedEvents));
    }

    [Fact]
    public async Task AddItemAsync_Should_Add_Item_And_Update_Total()
    {
        var context = CreateContext();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        var product = Product.Create("Laptop", 250_000m);
        await context.Customers.AddAsync(customer);
        await context.Products.AddAsync(product);
        var service = CreateService(context);
        var order = await service.CreateAsync(new CreateOrderRequest(customer.Id));

        var updatedOrder = await service.AddItemAsync(
            order.Id,
            new AddOrderItemRequest(product.Id, 2));

        var item = Assert.Single(updatedOrder.Items);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(product.Name, item.ProductName);
        Assert.Equal(500_000m, item.TotalPrice);
        Assert.Equal(500_000m, updatedOrder.TotalAmount);

        var savedOrder = await context.Orders.GetByIdAsync(order.Id);
        Assert.NotNull(savedOrder);
        Assert.Empty(savedOrder.DomainEvents);
        Assert.Contains(
            context.DomainEvents.DispatchedEvents,
            domainEvent => domainEvent is OrderItemAddedDomainEvent);
    }

    [Fact]
    public async Task ConfirmAsync_Should_Confirm_NonEmpty_Order()
    {
        var context = CreateContext();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        var product = Product.Create("Keyboard", 20_000m);
        await context.Customers.AddAsync(customer);
        await context.Products.AddAsync(product);
        var service = CreateService(context);
        var order = await service.CreateAsync(new CreateOrderRequest(customer.Id));

        await service.AddItemAsync(
            order.Id,
            new AddOrderItemRequest(product.Id, 1));

        var confirmedOrder = await service.ConfirmAsync(order.Id);

        Assert.Equal(OrderStatus.Confirmed, confirmedOrder.Status);
    }

    [Fact]
    public async Task ConfirmAsync_Should_Throw_When_Order_Is_Empty()
    {
        var context = CreateContext();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        await context.Customers.AddAsync(customer);
        var service = CreateService(context);
        var order = await service.CreateAsync(new CreateOrderRequest(customer.Id));

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

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Customer_Does_Not_Exist()
    {
        var service = CreateService();

        var action = () => service.CreateAsync(new CreateOrderRequest(Guid.NewGuid()));

        await Assert.ThrowsAsync<KeyNotFoundException>(action);
    }

    [Fact]
    public async Task AddItemAsync_Should_Throw_When_Product_Is_Unavailable()
    {
        var context = CreateContext();
        var customer = Customer.Register("Ada Lovelace", "ada@example.com");
        var product = Product.Create("Keyboard", 20_000m);
        product.MarkUnavailable();
        await context.Customers.AddAsync(customer);
        await context.Products.AddAsync(product);
        var service = CreateService(context);
        var order = await service.CreateAsync(new CreateOrderRequest(customer.Id));

        var action = () => service.AddItemAsync(
            order.Id,
            new AddOrderItemRequest(product.Id, 1));

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    private static OrderService CreateService() =>
        CreateService(CreateContext());

    private static OrderService CreateService(TestPersistenceContext context) =>
        new(
            context.Orders,
            context.Customers,
            context.Products,
            new OrderFactory(),
            context.DomainEvents,
            new ActiveCustomerSpecification(),
            new AvailableProductSpecification());

    private static TestPersistenceContext CreateContext() =>
        new(
            new FakeOrderRepository(),
            new FakeCustomerRepository(),
            new FakeProductRepository(),
            new FakeDomainEventDispatcher());

    private sealed record TestPersistenceContext(
        FakeOrderRepository Orders,
        FakeCustomerRepository Customers,
        FakeProductRepository Products,
        FakeDomainEventDispatcher DomainEvents);

    private sealed class FakeDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly List<IDomainEvent> _dispatchedEvents = [];

        public IReadOnlyCollection<IDomainEvent> DispatchedEvents => _dispatchedEvents.AsReadOnly();

        public Task DispatchAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default)
        {
            _dispatchedEvents.AddRange(domainEvents);

            return Task.CompletedTask;
        }
    }

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

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Dictionary<Guid, Product> _products = [];

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            _products.Add(product.Id, product);

            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            _products.TryGetValue(productId, out var product);

            return Task.FromResult(product);
        }

        public Task SaveAsync(Product product, CancellationToken cancellationToken = default)
        {
            _products[product.Id] = product;

            return Task.CompletedTask;
        }
    }
}
