using Ordering.Application.Orders;
using Ordering.Application.Services;

namespace Ordering.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/orders");

        orders.MapPost(
                "/",
                async (
                    CreateOrderRequest request,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.CreateAsync(request, cancellationToken);

                    return Results.Created($"/orders/{order.Id}", order);
                })
            .WithName("CreateOrder");

        orders.MapGet(
                "/{orderId:guid}",
                async (
                    Guid orderId,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.GetAsync(orderId, cancellationToken);

                    return Results.Ok(order);
                })
            .WithName("GetOrder");

        orders.MapPost(
                "/{orderId:guid}/items",
                async (
                    Guid orderId,
                    AddOrderItemRequest request,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.AddItemAsync(
                        orderId,
                        request,
                        cancellationToken);

                    return Results.Ok(order);
                })
            .WithName("AddOrderItem");

        orders.MapPatch(
                "/{orderId:guid}/items/{productId:guid}",
                async (
                    Guid orderId,
                    Guid productId,
                    ChangeOrderItemQuantityRequest request,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.ChangeItemQuantityAsync(
                        orderId,
                        productId,
                        request,
                        cancellationToken);

                    return Results.Ok(order);
                })
            .WithName("ChangeOrderItemQuantity");

        orders.MapDelete(
                "/{orderId:guid}/items/{productId:guid}",
                async (
                    Guid orderId,
                    Guid productId,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.RemoveItemAsync(
                        orderId,
                        productId,
                        cancellationToken);

                    return Results.Ok(order);
                })
            .WithName("RemoveOrderItem");

        orders.MapPost(
                "/{orderId:guid}/confirm",
                async (
                    Guid orderId,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.ConfirmAsync(orderId, cancellationToken);

                    return Results.Ok(order);
                })
            .WithName("ConfirmOrder");

        orders.MapPost(
                "/{orderId:guid}/cancel",
                async (
                    Guid orderId,
                    OrderService orderService,
                    CancellationToken cancellationToken) =>
                {
                    var order = await orderService.CancelAsync(orderId, cancellationToken);

                    return Results.Ok(order);
                })
            .WithName("CancelOrder");

        return app;
    }
}
