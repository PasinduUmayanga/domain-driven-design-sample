using Microsoft.AspNetCore.Diagnostics;
using Ordering.Api.Endpoints;
using Ordering.Application.Services;
using Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOrderingInfrastructure();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?
            .Error;

        return exception switch
        {
            KeyNotFoundException keyNotFoundException => Results.NotFound(
                    new { error = keyNotFoundException.Message })
                .ExecuteAsync(context),
            ArgumentException argumentException => Results.BadRequest(
                    new { error = argumentException.Message })
                .ExecuteAsync(context),
            InvalidOperationException invalidOperationException => Results.BadRequest(
                    new { error = invalidOperationException.Message })
                .ExecuteAsync(context),
            _ => Results.Problem().ExecuteAsync(context)
        };
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API v1");
        options.RoutePrefix = "swagger";
    });
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    // Keep this endpoint dependency-free so deployment probes can use it.
    .WithName("GetHealth");

app.MapOrderEndpoints();
app.MapCustomerEndpoints();
app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();
