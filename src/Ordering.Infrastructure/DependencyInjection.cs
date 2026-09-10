using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Infrastructure.Customers;
using Ordering.Infrastructure.Orders;
using Ordering.Infrastructure.Products;

namespace Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        return services;
    }
}
