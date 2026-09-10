using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Infrastructure.Orders;

namespace Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        return services;
    }
}
