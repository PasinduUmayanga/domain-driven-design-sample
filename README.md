# domain-driven-design-sample

[![Build status](https://ci.appveyor.com/api/projects/status/ph5kr4120pudw80n/branch/main?svg=true)](https://ci.appveyor.com/project/Mahadenamuththa/domain-driven-design-sample/branch/main)
[![Build History](https://img.shields.io/badge/AppVeyor-Build%20History-blue?logo=appveyor)](https://ci.appveyor.com/project/Mahadenamuththa/domain-driven-design-sample/history)

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4?logo=dotnet&logoColor=white)
![OpenAPI](https://img.shields.io/badge/Microsoft.AspNetCore.OpenApi-10.0.11-512BD4?logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-2.9.3-5A3E85)

Related links:

- [GitHub repository](https://github.com/PasinduUmayanga/domain-driven-design-sample)
- [AppVeyor project](https://ci.appveyor.com/project/PasinduUmayanga/domain-driven-design-sample)
- [AppVeyor build history](https://ci.appveyor.com/project/PasinduUmayanga/domain-driven-design-sample/history)

![image](https://github.com/user-attachments/assets/f97b2c1e-1f22-4d0c-8358-d4f1fdf0c23c)

# Step 1 — What is Domain-Driven Design?

Before coding, understand this idea.

DDD means:

Build your software around the business domain and business rules instead of around database tables.

For example

![image](https://github.com/user-attachments/assets/fead04cd-8239-4845-8a0b-96dda3f9de22)

## What is a Domain?

The domain is the business problem that the software is solving.

For our example:

Domain: Online Ordering

Inside that domain we have concepts such as:

![image](https://github.com/user-attachments/assets/9e3b50a8-eb71-434d-b63c-5dc981704fdf)

These aren't programming concepts.

They are business concepts.

DDD tries to represent these concepts directly in code.

For example:

`Order`

should behave like a real business order.

It shouldn't simply be:

```csharp
public class Order
{
    public int Id { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; }
}
```

This is mostly a data structure.

DDD wants something closer to:

```csharp
Order order = Order.Create(customerId);

order.AddItem(productId, price, quantity);

order.Confirm();
```

Now the model represents actual business behavior.

## Ubiquitous Language

This is one of the most important DDD ideas.

Developers and business people should use the same terminology.

For our ordering domain, the shared language includes:

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/3e2fc4ad-89c5-43aa-ade9-56f57ca5718b" />


The C# code should use exactly those concepts:

```csharp
order.AddItem(...);

order.RemoveItem(...);

order.Confirm();

order.Cancel();
```

Avoid vague technical method names like:

```csharp
UpdateData();

Process();

ModifyRecord();
```

Using the same words in conversations, requirements, tests, and code is part of what DDD calls Ubiquitous Language.

## First Business Rules

Before writing code, let's define our ordering rules.

We'll begin with these:

1. An order belongs to one customer.
2. An order starts in Pending status.
3. A customer can add products while the order is Pending.
4. Quantity must be greater than zero.
5. Product price cannot be negative.
6. A confirmed order cannot be modified.
7. A cancelled order cannot be modified.
8. An empty order cannot be confirmed.
9. Order total is calculated from its items.
10. An order can only be confirmed once.

# Step 2 — Create our .NET 10 solution
## Initial Architecture

I recommend this initial architecture:

<img width="1774" height="887" alt="image" src="https://github.com/user-attachments/assets/98831335-34f9-4a0d-81e9-c5996ba49afa" />


## Create the Solution in Visual Studio

You can create this solution using the Visual Studio user interface.

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/7ba36880-47e4-4bc2-ad28-01398eee502c" />

After the solution is created, add two solution folders:

1. Right-click the solution.
2. Select **Add**.
3. Select **New Solution Folder**.
4. Name the first folder `src`.
5. Repeat the same steps and name the second folder `tests`.

Now create projects:

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/0d33e752-ceb2-4a7a-b2db-73d2518aaa3f" />


Add project references


<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/5dc9cb14-fc4c-4eda-a741-936829ffa75a" />


Check the dependency structure

You should now have approximately:
<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/3e5bc502-61f9-4d3d-96d7-7efbf8087537" />

Why four projects?

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/4db9df5d-2552-4358-917d-de29cfd30652" />

Very important DDD dependency rule
Think about the architecture like this:

<img width="1145" height="1373" alt="image" src="https://github.com/user-attachments/assets/5e17ea6c-6efa-4200-bdda-e7d7c06c6663" />



And infrastructure supports it:
<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/52a86822-0d2b-4a55-88ee-a805ecffc5f2" />

But this should never happen:

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/3123ba35-59a2-4f3a-b576-ad6ace4efc0d" />

# Step 3 - Create the Order Entity

Inside `src/Ordering.Domain`, create this structure:

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/eaade010-f557-40ff-86fe-51b482570d54" />


Create `OrderStatus.cs`:

```csharp
namespace Ordering.Domain.Orders;

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3
}
```

Now create `Order.cs`:

```csharp
namespace Ordering.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Order()
    {
    }

    private Order(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Order Create(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));
        }

        return new Order(
            Guid.NewGuid(),
            customerId);
    }
}
```

### Why is Order an Entity?

In DDD, an Entity is identified by its unique identity, usually an ID, rather than only by its data.

```text
Order A -> Id: 111
Order B -> Id: 222
```

Even if both orders have the same customer, total, and status, they are still different orders because their IDs are different.

```text
Entity = Identity matters
```

In contrast, a Value Object is identified by its values.

```text
LKR 500 == LKR 500
```

```text
Value Object = Values matter
```

So, `Order` is an Entity because each order has its own unique identity.

### Why Use Private Setters?

In DDD, entities should protect their own state.

Public setters allow code outside the entity to change its state directly:

```csharp
public OrderStatus Status { get; set; }
```

That means any code can do this:

```csharp
order.Status = OrderStatus.Confirmed;
order.CustomerId = Guid.Empty;
```

This can allow invalid state changes.

Private setters prevent direct changes from outside the entity:

```csharp
public OrderStatus Status { get; private set; }
```

Now the state must be changed through business methods:

```csharp
order.Confirm();
```

The method can validate and enforce business rules before changing the state.

Encapsulation means the entity protects and controls its own state.

### Why Use Order.Create()?

In DDD, an entity should be valid from the moment it is created.

Direct creation can bypass important rules:

```csharp
var order = new Order();
```

This could allow an invalid order:

```csharp
order.CustomerId = Guid.Empty;
```

Instead, use a factory method:

```csharp
var order = Order.Create(customerId);
```

The `Create()` method validates required data and creates the entity correctly:

```csharp
public static Order Create(Guid customerId)
{
    if (customerId == Guid.Empty)
    {
        throw new ArgumentException(
            "Customer ID cannot be empty.",
            nameof(customerId));
    }

    return new Order(
        Guid.NewGuid(),
        customerId);
}
```

This ensures every new `Order` starts with:

- A valid Order ID
- A valid Customer ID
- The correct initial status
- A created date

DDD principle: make invalid states difficult or impossible to create.

### Why Is the Constructor Private?

A private constructor prevents outside code from creating an `Order` directly.

Avoid direct creation:

```csharp
new Order(...);
```

Use the factory method instead:

```csharp
var order = Order.Create(customerId);
```

This ensures the Domain controls how an `Order` is created and can enforce all required business rules.

```csharp
private Order(Guid id, Guid customerId)
{
    // Initialize valid Order
}
```

### What About the Empty Constructor?

```csharp
private Order()
{
}
```

This will later be used by EF Core when loading an `Order` from the database.

Application code should normally create orders through:

```csharp
Order.Create(customerId);
```

Key idea: private constructors prevent uncontrolled object creation and help keep the entity valid.

## Step 4 - Add Behaviour to the Entity

In DDD, an Entity should contain both data and business behaviour.

Instead of directly changing:

```csharp
order.Status = OrderStatus.Confirmed;
```

We expose meaningful business operations:

```csharp
order.Confirm();
order.Cancel();
```

### Add Business Behaviour

```csharp
public void Confirm()
{
    if (Status != OrderStatus.Pending)
    {
        throw new InvalidOperationException(
            "Only pending orders can be confirmed.");
    }

    Status = OrderStatus.Confirmed;
}

public void Cancel()
{
    if (Status != OrderStatus.Pending)
    {
        throw new InvalidOperationException(
            "Only pending orders can be cancelled.");
    }

    Status = OrderStatus.Cancelled;
}
```

These methods protect the Entity by enforcing business rules.

For example:

```text
Pending -> Confirmed   OK
Pending -> Cancelled   OK
Confirmed -> Cancelled Not allowed
Cancelled -> Confirmed Not allowed
```

### Usage

```csharp
var order = Order.Create(customerId);

order.Confirm();
```

### Why Is This Important?

Data-focused code changes a value directly:

```csharp
order.Status = OrderStatus.Confirmed;
```

That means:

```text
Set the status value.
```

DDD behaviour-focused code uses a business operation:

```csharp
order.Confirm();
```

That means:

```text
Confirm the order.
```

This makes the code reflect the business language and keeps business rules inside the Domain.

DDD principle: Entities should not just store data. They should contain the business behaviour that controls how their state can change.

### Business Rule Example - Domain Invariant

In DDD, an Entity should protect its business rules.

Consider:

```csharp
var order = Order.Create(customerId);

order.Confirm();
order.Cancel(); // Exception
```

After `Confirm()`:

```text
Status = Confirmed
```

But our `Cancel()` rule requires:

```text
Only Pending orders can be cancelled.
```

Therefore, calling `Cancel()` throws:

```text
Only pending orders can be cancelled.
```

### What Is a Domain Invariant?

A Domain Invariant is:

```text
A business rule that must always remain true.
```

For our `Order`:

```text
Pending -> Confirmed   OK
Pending -> Cancelled   OK
Confirmed -> Cancelled Not allowed
```

The `Order` Entity itself enforces these rules.

DDD principle: keep important business rules inside the Domain so an Entity cannot enter an invalid state.

# Step 5 - Add Order Items and Totals

The order aggregate is complete when it can protect both its lifecycle and its line items.

Add `OrderItem.cs` inside `src/Ordering.Domain/Orders`.

An order item captures:

- Product ID
- Product name
- Unit price
- Quantity
- Line total

The item validates its own rules:

- Product ID cannot be empty.
- Product name cannot be empty.
- Unit price cannot be negative.
- Quantity must be greater than zero.

The `Order` aggregate owns the item collection. Code outside the aggregate can read `Items`, but it cannot directly replace or mutate the collection.

```csharp
private readonly List<OrderItem> _items = [];

public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

public decimal TotalAmount =>
    _items.Sum(item => item.TotalPrice);
```

Expose behaviour instead of setters:

```csharp
order.AddItem(productId, "Laptop", 250_000m, 2);

order.ChangeItemQuantity(productId, 3);

order.RemoveItem(productId);
```

This keeps the business rules inside the aggregate. A confirmed or cancelled order cannot be modified because each item-changing operation checks that the order is still pending.

# Step 6 - Add the Application Layer

The Application layer contains use cases. It coordinates the domain model but does not contain HTTP or database code.

Create this structure:

```text
src/Ordering.Application
|-- Abstractions
|   `-- Persistence
|       `-- IOrderRepository.cs
|-- Orders
|   |-- AddOrderItemRequest.cs
|   |-- ChangeOrderItemQuantityRequest.cs
|   |-- CreateOrderRequest.cs
|   |-- OrderItemResponse.cs
|   `-- OrderResponse.cs
`-- Services
    `-- OrderService.cs
```

`IOrderRepository` is a port owned by the Application layer:

```csharp
public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
}
```

The important Clean Architecture rule is:

```text
Application defines what it needs.
Infrastructure decides how to provide it.
```

`OrderService` uses the repository abstraction and the domain aggregate:

```csharp
var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

order.AddItem(
    request.ProductId,
    request.ProductName,
    request.UnitPrice,
    request.Quantity);

await orderRepository.SaveAsync(order, cancellationToken);
```

The service does not calculate totals itself. It asks the domain model to perform business behaviour, then returns a response DTO.

# Step 7 - Add the Infrastructure Layer

Infrastructure contains technical adapters.

For this sample, use an in-memory repository so the application can run without a database:

```text
src/Ordering.Infrastructure
|-- DependencyInjection.cs
`-- Orders
    `-- InMemoryOrderRepository.cs
```

`InMemoryOrderRepository` implements the Application layer's `IOrderRepository` port.

The Infrastructure project may reference Application and Domain:

```text
Ordering.Infrastructure -> Ordering.Application
Ordering.Infrastructure -> Ordering.Domain
```

But Application must not reference Infrastructure.

Add a dependency registration method in `DependencyInjection.cs`:

```csharp
public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services)
{
    services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

    return services;
}
```

# Step 8 - Add API Endpoints

The API project is the composition root. It wires dependencies and exposes HTTP endpoints.

Keep `Program.cs` small:

```csharp
builder.Services.AddOrderingInfrastructure();
builder.Services.AddScoped<OrderService>();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapOrderEndpoints();
```

Put order endpoint mappings in a separate file:

```text
src/Ordering.Api
|-- Endpoints
|   `-- OrderEndpoints.cs
`-- Program.cs
```

`OrderEndpoints.cs` maps the HTTP workflow:

```http
POST /orders
GET /orders/{orderId}
POST /orders/{orderId}/items
PATCH /orders/{orderId}/items/{productId}
DELETE /orders/{orderId}/items/{productId}
POST /orders/{orderId}/confirm
POST /orders/{orderId}/cancel
```

The endpoint file should call `OrderService`; it should not contain business rules. For example, the endpoint asks the service to confirm the order, and the `Order` aggregate decides whether confirmation is valid.

# Step 9 - Add Tests

Tests should follow the layer being tested.

Domain tests verify business rules directly:

- Creating an order starts in `Pending`.
- Empty orders cannot be confirmed.
- Confirmed orders cannot be cancelled.
- Cancelled orders cannot be confirmed.
- Confirmed or cancelled orders cannot be modified.
- Totals come from order items.

Application tests verify orchestration:

- `CreateAsync` creates a pending order.
- `AddItemAsync` saves item changes and returns updated totals.
- `ConfirmAsync` confirms a non-empty order.
- Missing orders throw `KeyNotFoundException`.

Application tests use a fake repository inside the test project. They should not depend on Infrastructure.

# Step 10 - Run and Verify

Build the solution:

```powershell
dotnet build OrderingSystem.slnx
```

Run the tests:

```powershell
dotnet test OrderingSystem.slnx
```

Run the API:

```powershell
dotnet run --project src/Ordering.Api
```

Check the health endpoint:

```http
GET /health
```

# Step 11 - Complete Ordering Workflow

The sample now includes a small end-to-end order workflow around the domain model:

- `Ordering.Domain` owns the `Order` aggregate, item rules, totals, and lifecycle transitions.
- `Ordering.Application` exposes use cases through `Services/OrderService` and owns the `IOrderRepository` port under `Abstractions/Persistence`.
- `Ordering.Infrastructure` provides the in-memory repository adapter for the sample.
- `Ordering.Api` is the composition root and exposes HTTP endpoints for creating, editing, confirming, and cancelling orders.

The API uses Infrastructure's in-memory repository so the sample can run without a database while still keeping dependencies pointed inward.

## Run the API

From the repository root:

```powershell
dotnet run --project src/Ordering.Api
```

Check the health endpoint:

```http
GET /health
```

## Order Endpoints

Create an order:

```http
POST /orders
Content-Type: application/json

{
  "customerId": "11111111-1111-1111-1111-111111111111"
}
```

Add an item:

```http
POST /orders/{orderId}/items
Content-Type: application/json

{
  "productId": "22222222-2222-2222-2222-222222222222",
  "productName": "Laptop",
  "unitPrice": 250000,
  "quantity": 2
}
```

Change an item quantity:

```http
PATCH /orders/{orderId}/items/{productId}
Content-Type: application/json

{
  "quantity": 3
}
```

Complete the order lifecycle:

```http
POST /orders/{orderId}/confirm
POST /orders/{orderId}/cancel
```

Important rules are still enforced by the domain model. Empty orders cannot be confirmed, non-pending orders cannot be modified, quantities must be positive, and prices cannot be negative.
