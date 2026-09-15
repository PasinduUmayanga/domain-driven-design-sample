using System.Reflection;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Common;

namespace Ordering.Application.Tests;

public class RepositoryArchitectureTests
{
    [Fact]
    public void Repository_interfaces_Should_Only_Expose_Aggregate_Roots()
    {
        var repositoryInterfaces = typeof(IOrderRepository)
            .Assembly
            .GetTypes()
            .Where(type =>
                type.IsInterface
                && type.Namespace == "Ordering.Application.Abstractions.Persistence"
                && type.Name.EndsWith("Repository", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(repositoryInterfaces);

        foreach (var repositoryInterface in repositoryInterfaces)
        {
            var domainTypes = repositoryInterface
                .GetMethods()
                .SelectMany(GetDomainTypes)
                .Distinct()
                .ToArray();

            Assert.NotEmpty(domainTypes);

            Assert.All(
                domainTypes,
                domainType => Assert.True(
                    typeof(AggregateRoot).IsAssignableFrom(domainType),
                    $"{repositoryInterface.Name} exposes {domainType.Name}, but repositories should expose aggregate roots only."));
        }
    }

    private static IEnumerable<Type> GetDomainTypes(MethodInfo method)
    {
        foreach (var parameter in method.GetParameters())
        {
            if (IsDomainType(parameter.ParameterType))
            {
                yield return parameter.ParameterType;
            }
        }

        var returnType = UnwrapTask(method.ReturnType);

        if (Nullable.GetUnderlyingType(returnType) is { } nullableType)
        {
            returnType = nullableType;
        }

        if (IsDomainType(returnType))
        {
            yield return returnType;
        }
    }

    private static Type UnwrapTask(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return type.GetGenericArguments()[0];
        }

        return type;
    }

    private static bool IsDomainType(Type type) =>
        type.Namespace?.StartsWith("Ordering.Domain.", StringComparison.Ordinal) == true;
}
