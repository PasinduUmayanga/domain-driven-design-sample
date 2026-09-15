using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Abstractions.DomainEvents;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Specifications;
using Ordering.Application.Specifications.Customers;
using Ordering.Application.Specifications.Products;
using Ordering.Domain.Customers;
using Ordering.Domain.Products;
using Ordering.Infrastructure.Customers;
using Ordering.Infrastructure.DomainEvents;
using Ordering.Infrastructure.Orders;
using Ordering.Infrastructure.Products;

namespace Ordering.Infrastructure;

/// <summary>
/// Provides dependency injection registrations for infrastructure adapters.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure implementations for application ports and learning-focused DDD patterns.
    /// </summary>
    /// <param name="services">The service collection used by the API composition root.</param>
    /// <returns>The same service collection so registrations can be chained.</returns>
    public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services)
    {
        #region Persistence

        // Repositories are registered through application interfaces so the
        // Application layer does not depend on Infrastructure.
        services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        #endregion

        #region Domain Events

        // This sample logs domain events after persistence. A production system
        // could replace this with handlers, an outbox, or a message bus publisher.
        services.AddSingleton<IDomainEventDispatcher, LoggingDomainEventDispatcher>();

        #endregion

        #region Specifications

        // Specifications give important cross-aggregate rules a name and keep
        // application services readable.
        services.AddSingleton<ISpecification<Customer>, ActiveCustomerSpecification>();
        services.AddSingleton<ISpecification<Product>, AvailableProductSpecification>();

        #endregion

        return services;
    }
}
