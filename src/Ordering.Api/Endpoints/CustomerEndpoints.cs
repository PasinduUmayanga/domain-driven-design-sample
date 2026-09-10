using Ordering.Application.Customers;
using Ordering.Application.Services;

namespace Ordering.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/customers");

        customers.MapPost(
                "/",
                async (
                    CreateCustomerRequest request,
                    CustomerService customerService,
                    CancellationToken cancellationToken) =>
                {
                    var customer = await customerService.RegisterAsync(request, cancellationToken);

                    return Results.Created($"/customers/{customer.Id}", customer);
                })
            .WithName("CreateCustomer");

        customers.MapGet(
                "/{customerId:guid}",
                async (
                    Guid customerId,
                    CustomerService customerService,
                    CancellationToken cancellationToken) =>
                {
                    var customer = await customerService.GetAsync(customerId, cancellationToken);

                    return Results.Ok(customer);
                })
            .WithName("GetCustomer");

        customers.MapPut(
                "/{customerId:guid}",
                async (
                    Guid customerId,
                    UpdateCustomerRequest request,
                    CustomerService customerService,
                    CancellationToken cancellationToken) =>
                {
                    var customer = await customerService.UpdateAsync(
                        customerId,
                        request,
                        cancellationToken);

                    return Results.Ok(customer);
                })
            .WithName("UpdateCustomer");

        customers.MapPost(
                "/{customerId:guid}/deactivate",
                async (
                    Guid customerId,
                    CustomerService customerService,
                    CancellationToken cancellationToken) =>
                {
                    var customer = await customerService.DeactivateAsync(
                        customerId,
                        cancellationToken);

                    return Results.Ok(customer);
                })
            .WithName("DeactivateCustomer");

        return app;
    }
}
