var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    // Keep this endpoint dependency-free so deployment probes can use it.
    .WithName("GetHealth");

app.UseHttpsRedirection();

app.Run();
