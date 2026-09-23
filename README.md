# Domain-Driven Design Sample

[![Build status](https://ci.appveyor.com/api/projects/status/ph5kr4120pudw80n/branch/main?svg=true)](https://ci.appveyor.com/project/Mahadenamuththa/domain-driven-design-sample/branch/main)
[![Build History](https://img.shields.io/badge/AppVeyor-Build%20History-blue?logo=appveyor)](https://ci.appveyor.com/project/Mahadenamuththa/domain-driven-design-sample/history)

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4?logo=dotnet&logoColor=white)
![OpenAPI](https://img.shields.io/badge/Microsoft.AspNetCore.OpenApi-10.0.11-512BD4?logo=dotnet&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-Swashbuckle-85EA2D)
![xUnit](https://img.shields.io/badge/xUnit-2.9.3-5A3E85)

This repository is a learning-focused .NET sample for Domain-Driven Design (DDD), Clean Architecture, aggregate roots, domain events, and a simple ordering API.

## Table of Contents

- [Purpose](#purpose)
- [Solution Structure](#solution-structure)
- [Architecture Rules](#architecture-rules)
- [Domain Model](#domain-model)
- [Value Objects](#value-objects)
- [Aggregate Roots](#aggregate-roots)
- [Factories](#factories)
- [Domain Events](#domain-events)
- [Specification Pattern](#specification-pattern)
- [Business Rules](#business-rules)
- [Application Layer](#application-layer)
- [Infrastructure Layer](#infrastructure-layer)
- [API Layer](#api-layer)
- [Run the Project](#run-the-project)
- [Swagger](#swagger)
- [Postman Scenario](#postman-scenario)
- [Testing](#testing)
- [Useful Links](#useful-links)

## Purpose

DDD means building software around the business domain and business rules instead of starting from database tables.

This sample uses an online ordering domain:

- `Customer` - a person who can place orders.
- `Product` - an item that can be added to an order.
- `Order` - a customer's purchase request.
- `OrderItem` - a product line captured inside an order.

The goal is to make the code read like the business workflow:

```csharp
var order = Order.Create(customerId);

order.AddItem(product.Id, product.Name, product.UnitPrice, quantity);

order.Confirm();
```

Avoid data-only models such as:

```csharp
public class Order
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}
```

The domain model should protect its own rules through meaningful behavior.

## Solution Structure

```text
domain-driven-design-sample
|-- src
|   |-- Ordering.Api
|   |-- Ordering.Application
|   |-- Ordering.Domain
|   `-- Ordering.Infrastructure
|-- tests
|   |-- Ordering.Application.Tests
|   `-- Ordering.Domain.Tests
|-- OrderingSystem.slnx
`-- README.md
```

Project responsibilities:

| Project | Responsibility |
| --- | --- |
| `Ordering.Domain` | Business concepts, entities, aggregate roots, rules, and domain events. |
| `Ordering.Application` | Use cases, request/response DTOs, and repository interfaces. |
| `Ordering.Infrastructure` | Technical adapters such as in-memory repositories. |
| `Ordering.Api` | HTTP endpoints, dependency injection, Swagger, and app startup. |
| `Ordering.Domain.Tests` | Direct domain rule tests. |
| `Ordering.Application.Tests` | Use-case orchestration tests with fake repositories. |

### How to implement this structure

1. Create the source projects:
   - `src/Ordering.Domain`
   - `src/Ordering.Application`
   - `src/Ordering.Infrastructure`
   - `src/Ordering.Api`
2. Create the test projects:
   - `tests/Ordering.Domain.Tests`
   - `tests/Ordering.Application.Tests`
3. Add project references:
   - `Ordering.Application` references `Ordering.Domain`.
   - `Ordering.Infrastructure` references `Ordering.Application` and `Ordering.Domain`.
   - `Ordering.Api` references `Ordering.Application` and `Ordering.Infrastructure`.
   - Test projects reference the projects they test.
4. Keep the dependency rule:
   - Domain depends on nothing.
   - Application depends on Domain.
   - Infrastructure depends on Application and Domain.
   - API is the composition root.

## Architecture Rules

Dependencies should point inward:

```text
Ordering.Api
    -> Ordering.Application
    -> Ordering.Infrastructure

Ordering.Infrastructure
    -> Ordering.Application
    -> Ordering.Domain

Ordering.Application
    -> Ordering.Domain

Ordering.Domain
    -> no project dependencies
```

Important rules:

- Domain must not depend on API, Infrastructure, or Application.
- Application defines repository interfaces.
- Infrastructure implements repository interfaces.
- API calls application services, not domain entities directly.
- Business rules live in the Domain project.

## Domain Model

The main domain model lives in `src/Ordering.Domain`.

```text
src/Ordering.Domain
|-- Common
|   |-- ValueObjects
|   |   |-- Email.cs
|   |   |-- Money.cs
|   |   |-- ProductName.cs
|   |   `-- Quantity.cs
|   |-- AggregateRoot.cs
|   |-- Entity.cs
|   `-- IDomainEvent.cs
|-- Customers
|   `-- Customer.cs
|-- Orders
|   |-- Events
|   |   |-- OrderCancelledDomainEvent.cs
|   |   |-- OrderConfirmedDomainEvent.cs
|   |   |-- OrderCreatedDomainEvent.cs
|   |   `-- OrderItemAddedDomainEvent.cs
|   |-- IOrderFactory.cs
|   |-- Order.cs
|   |-- OrderFactory.cs
|   |-- OrderItem.cs
|   `-- OrderStatus.cs
`-- Products
    `-- Product.cs
```

### How to implement the domain building blocks

1. Add `src/Ordering.Domain/Common/Entity.cs`.
   - Put the shared `Id` property here.
2. Add `src/Ordering.Domain/Common/AggregateRoot.cs`.
   - Inherit from `Entity`.
   - Store pending domain events here.
3. Add `src/Ordering.Domain/Common/IDomainEvent.cs`.
   - Add `OccurredAtUtc`.
4. Update aggregate roots to inherit from `AggregateRoot`.
   - `Customer`
   - `Product`
   - `Order`
5. Update child entities to inherit from `Entity`.
   - `OrderItem`
6. Add architecture tests in `tests/Ordering.Domain.Tests`.
   - Verify aggregate roots inherit from `AggregateRoot`.
   - Verify `OrderItem` is an entity inside the `Order` aggregate.

### Entity

An entity has identity. Two entities can have similar values but still be different objects because their IDs are different.

```text
Order A -> Id: 111
Order B -> Id: 222
```

`Entity` is the base type for domain objects with identity:

```csharp
public abstract class Entity
{
    public Guid Id { get; protected set; }
}
```

### Encapsulation

Domain objects protect state with private setters and business methods.

Prefer:

```csharp
order.Confirm();
```

Avoid:

```csharp
order.Status = OrderStatus.Confirmed;
```

The method name represents business language and gives the domain model a place to enforce rules.

## Value Objects

A value object is identified by its values, not by a separate identity.

Examples:

```text
Email("ada@example.com") == Email("ada@example.com")
Money(250000) == Money(250000)
Quantity(2) == Quantity(2)
```

This sample includes these value objects:

| Value Object | Purpose |
| --- | --- |
| `Email` | Validates and stores a customer email address. |
| `Money` | Ensures price and money amounts are not negative. |
| `ProductName` | Ensures product names are not empty and trims input. |
| `Quantity` | Ensures item quantities are greater than zero. |

Value objects keep small rules close to the values they protect. For example, `Quantity` owns the rule that quantity must be positive:

```csharp
public readonly record struct Quantity
{
    public int Value { get; }

    public static Quantity Create(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Quantity must be greater than zero.");
        }

        return new Quantity(value);
    }
}
```

The API still accepts simple JSON values such as strings, decimals, and integers. The Domain layer converts those values into value objects before storing or using them.

### How to implement value objects

1. Add value object files under `src/Ordering.Domain/Common/ValueObjects`:
   - `Email.cs`
   - `Money.cs`
   - `ProductName.cs`
   - `Quantity.cs`
2. Put validation inside each value object:
   - `Email` validates email format.
   - `Money` rejects negative amounts.
   - `ProductName` rejects empty names and trims input.
   - `Quantity` requires a value greater than zero.
3. Update `Customer`.
   - Store email using the `Email` value object.
   - Keep the public response-friendly property as `string Email`.
4. Update `Product`.
   - Store name using `ProductName`.
   - Store price using `Money`.
   - Keep public properties as `string Name` and `decimal UnitPrice`.
5. Update `OrderItem`.
   - Store captured product name using `ProductName`.
   - Store unit price using `Money`.
   - Store quantity using `Quantity`.
6. Do not add value objects to dependency injection.
   - Value objects are created directly in the Domain layer.
   - They are not services.
7. Add tests under `tests/Ordering.Domain.Tests/ValueObjects`.
   - Test valid creation.
   - Test invalid input.
   - Test equality where useful.

## Aggregate Roots

An aggregate is a consistency boundary. An aggregate root is the object that outside code is allowed to load, save, and call.

In this sample:

- `Customer` is an aggregate root.
- `Product` is an aggregate root.
- `Order` is an aggregate root.
- `OrderItem` is an entity inside the `Order` aggregate.

Use one project for the whole Domain layer. Do not create a separate project for each aggregate root.

Correct usage:

```csharp
var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

order.AddItem(product.Id, product.Name, product.UnitPrice, quantity);

await orderRepository.SaveAsync(order, cancellationToken);
```

Avoid:

```csharp
await orderItemRepository.SaveAsync(orderItem, cancellationToken);
```

`OrderItem` does not get its own repository because it is controlled by `Order`.

### How to implement aggregate roots and entities

1. Add `Customer` in `src/Ordering.Domain/Customers`.
   - Use `Register`.
   - Use methods like `UpdateProfile` and `Deactivate`.
2. Add `Product` in `src/Ordering.Domain/Products`.
   - Use `Create`.
   - Use methods like `Rename`, `ChangePrice`, `MarkUnavailable`, and `MarkAvailable`.
3. Add `Order` in `src/Ordering.Domain/Orders`.
   - Use `Create`.
   - Use methods like `AddItem`, `ChangeItemQuantity`, `RemoveItem`, `Confirm`, and `Cancel`.
4. Add `OrderItem` inside `src/Ordering.Domain/Orders`.
   - Keep its constructor `internal`.
   - Create and mutate order items only through `Order`.
5. Add repository interfaces only for aggregate roots.
   - `ICustomerRepository`
   - `IProductRepository`
   - `IOrderRepository`
6. Do not add `IOrderItemRepository`.
   - `OrderItem` belongs to the `Order` aggregate.

## Factories

A factory creates a domain object in one valid starting state. It is useful when aggregate creation starts to involve multiple steps, default values, domain events, or creation rules.

This sample shows both styles:

- `Order.Create(customerId)` is a factory method on the aggregate.
- `OrderFactory` is a domain factory service used by the Application layer.

The aggregate still protects its own invariants. The factory gives application code one clear place to ask for a new `Order`:

```csharp
var order = orderFactory.Create(customer.Id);
```

For a more complete creation scenario, the factory can create the order and add the first item in one call:

```csharp
var order = orderFactory.CreateWithItem(
    customer.Id,
    product.Id,
    product.Name,
    product.UnitPrice,
    quantity);
```

### How to implement factories

1. Keep aggregate constructors private or internal.
   - Outside code should not create invalid aggregate state with `new Order(...)`.
2. Add a factory method on the aggregate for simple creation.
   - `Order.Create(customerId)` validates the customer ID.
   - It sets the initial status to `Pending`.
   - It records `OrderCreatedDomainEvent`.
3. Add a domain factory interface beside the aggregate.
   - `src/Ordering.Domain/Orders/IOrderFactory.cs`
   - This lets application services depend on a named creation concept.
   - Add `Create(customerId)` for basic order creation.
   - Add `CreateWithItem(...)` for creating an order with its first line item.
4. Add the factory implementation beside the aggregate.
   - `src/Ordering.Domain/Orders/OrderFactory.cs`
   - It delegates to `Order.Create(customerId)` so the aggregate remains the owner of its invariants.
   - It can compose multiple aggregate methods, such as `Order.Create(...)` followed by `order.AddItem(...)`.
5. Inject the factory into the application service.
   - `OrderService` uses `IOrderFactory` when handling `CreateOrderRequest`.
   - The service stays focused on loading the customer, checking specifications, saving, and dispatching events.
6. Register the factory in dependency injection.
   - Add `services.AddSingleton<IOrderFactory, OrderFactory>()` in `AddOrderingInfrastructure`.
   - The factory type lives in Domain, but the registration belongs in the composition setup.
7. Add factory tests.
   - Test valid order creation.
   - Test invalid creation input, such as an empty customer ID.

## Domain Events

A domain event describes something important that already happened in the domain.

`AggregateRoot` stores pending domain events:

```csharp
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

`Order` records events after successful business operations:

- `OrderCreatedDomainEvent`
- `OrderItemAddedDomainEvent`
- `OrderConfirmedDomainEvent`
- `OrderCancelledDomainEvent`

Application services dispatch pending events after persistence succeeds, then clear them from the aggregate:

```csharp
order.Confirm();

await orderRepository.SaveAsync(order, cancellationToken);

await domainEventDispatcher.DispatchAsync(
    order.DomainEvents.ToArray(),
    cancellationToken);

order.ClearDomainEvents();
```

This sample uses `IDomainEventDispatcher` in the Application layer and `LoggingDomainEventDispatcher` in Infrastructure. The logging dispatcher is intentionally simple: it shows where event publishing belongs without adding a message broker or background worker.

### How to implement domain events

1. Add domain event records under `src/Ordering.Domain/Orders/Events`:
   - `OrderCreatedDomainEvent`
   - `OrderItemAddedDomainEvent`
   - `OrderConfirmedDomainEvent`
   - `OrderCancelledDomainEvent`
2. Raise events inside `Order` after successful state changes.
   - Raise `OrderCreatedDomainEvent` after creating an order.
   - Raise `OrderItemAddedDomainEvent` after adding or increasing an item.
   - Raise `OrderConfirmedDomainEvent` after confirming an order.
   - Raise `OrderCancelledDomainEvent` after cancelling an order.
3. Store events in `AggregateRoot`.
   - Use `RaiseDomainEvent`.
   - Expose `DomainEvents`.
   - Add `ClearDomainEvents`.
4. Do not dispatch events from the Domain layer.
   - The Domain records what happened.
   - The Application/Infrastructure layers decide how to publish it.
5. Add tests in `tests/Ordering.Domain.Tests/Orders`.
   - Verify each operation records the expected event.
   - Verify invalid operations do not record events.
   - Verify events can be cleared.

### How to implement domain event dispatching

1. Add `src/Ordering.Application/Abstractions/DomainEvents/IDomainEventDispatcher.cs`.
   - This is the Application-layer abstraction for publishing domain events.
2. Add `src/Ordering.Infrastructure/DomainEvents/LoggingDomainEventDispatcher.cs`.
   - This Infrastructure implementation logs dispatched events.
   - A real project could replace it with MediatR, a message bus, or an outbox.
3. Register the dispatcher in `src/Ordering.Infrastructure/DependencyInjection.cs`.
   - Add `IDomainEventDispatcher`.
   - Map it to `LoggingDomainEventDispatcher`.
4. Inject `IDomainEventDispatcher` into `OrderService`.
5. After repository save/add succeeds, dispatch and clear events:

   ```csharp
   await orderRepository.SaveAsync(order, cancellationToken);
   await domainEventDispatcher.DispatchAsync(order.DomainEvents.ToArray(), cancellationToken);
   order.ClearDomainEvents();
   ```

6. Add application tests.
   - Use a fake dispatcher.
   - Verify events are dispatched.
   - Verify aggregate events are cleared.

## Specification Pattern

The Specification Pattern gives a business rule a clear name and a reusable object.

This sample uses specifications for rules that are checked after loading another aggregate:

- `ActiveCustomerSpecification` answers whether a customer can place an order.
- `AvailableProductSpecification` answers whether a product can be added to an order.

The service code reads like the business rule:

```csharp
if (!activeCustomerSpecification.IsSatisfiedBy(customer))
{
    throw new InvalidOperationException(
        "Inactive customers cannot place orders.");
}

if (!availableProductSpecification.IsSatisfiedBy(product))
{
    throw new InvalidOperationException(
        "Unavailable products cannot be added to orders.");
}
```

Use specifications when a rule is important enough to name, test, and reuse. Keep simple aggregate invariants inside the aggregate itself.

### How to implement the Specification Pattern

1. Add `src/Ordering.Application/Specifications/ISpecification.cs`.
   - Define `bool IsSatisfiedBy(T candidate)`.
2. Add customer specifications.
   - `src/Ordering.Application/Specifications/Customers/ActiveCustomerSpecification.cs`
3. Add product specifications.
   - `src/Ordering.Application/Specifications/Products/AvailableProductSpecification.cs`
4. Register specifications in `src/Ordering.Infrastructure/DependencyInjection.cs`.
   - Register `ISpecification<Customer>` as `ActiveCustomerSpecification`.
   - Register `ISpecification<Product>` as `AvailableProductSpecification`.
5. Inject specifications into `OrderService`.
6. Replace inline cross-aggregate checks:
   - Replace direct `customer.IsActive` checks with `ActiveCustomerSpecification`.
   - Replace direct `product.IsAvailable` checks with `AvailableProductSpecification`.
7. Keep aggregate invariants inside aggregates.
   - Example: pending-order checks stay in `Order`.
8. Add tests in `tests/Ordering.Application.Tests/Specifications`.
   - Verify active/inactive customer results.
   - Verify available/unavailable product results.

## Business Rules

The sample enforces these rules:

1. A customer starts active.
2. Customer name and email are required.
3. Customer email must use a valid email format.
4. Inactive customers cannot be modified.
5. A product name is required.
6. Product price cannot be negative.
7. Products can be marked available or unavailable.
8. An order belongs to one customer.
9. An order starts in `Pending` status.
10. A customer must be active before placing an order.
11. A product must be available before it can be added to an order.
12. Quantity must be greater than zero.
13. Product price cannot be negative.
14. Orders capture product name and price at the time the item is added.
15. Order total is calculated from its items.
16. Empty orders cannot be confirmed.
17. Confirmed orders cannot be modified.
18. Cancelled orders cannot be modified.
19. An order can only be confirmed once.

## Application Layer

The Application layer contains use cases. It coordinates the domain model but does not contain HTTP or database code.

```text
src/Ordering.Application
|-- Abstractions
|   `-- Persistence
|       |-- ICustomerRepository.cs
|       |-- IOrderRepository.cs
|       `-- IProductRepository.cs
|-- Customers
|-- Orders
|-- Products
|-- Specifications
`-- Services
    |-- CustomerService.cs
    |-- OrderService.cs
    `-- ProductService.cs
```

Application services:

- Load aggregate roots from repositories.
- Call domain methods.
- Save aggregate roots.
- Return response DTOs.

Example:

```csharp
var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

order.Confirm();

await orderRepository.SaveAsync(order, cancellationToken);
```

Application services should not duplicate domain rules. For example, `OrderService` should ask `Order` to confirm itself; `Order` decides whether that operation is valid.

### How to implement the Application layer

1. Add request/response DTOs in `Ordering.Application`.
   - `Customers`
   - `Products`
   - `Orders`
2. Add repository interfaces under `src/Ordering.Application/Abstractions/Persistence`.
   - These are application ports.
3. Add application services under `src/Ordering.Application/Services`.
   - `CustomerService`
   - `ProductService`
   - `OrderService`
4. In application services:
   - Load aggregate roots from repositories.
   - Call domain methods.
   - Save aggregate roots.
   - Return response DTOs.
5. Do not put HTTP logic in Application.
6. Do not put database implementation logic in Application.

## Infrastructure Layer

Infrastructure contains technical adapters.

Current adapters:

- `InMemoryCustomerRepository`
- `InMemoryProductRepository`
- `InMemoryOrderRepository`

These repositories are intentionally simple so the sample can run without a database.

Registration happens in `DependencyInjection.cs`:

```csharp
public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services)
{
    services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
    services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
    services.AddSingleton<IProductRepository, InMemoryProductRepository>();

    return services;
}
```

### How to implement the Infrastructure layer

1. Add in-memory repositories:
   - `InMemoryCustomerRepository`
   - `InMemoryProductRepository`
   - `InMemoryOrderRepository`
2. Implement Application repository interfaces.
3. Add `src/Ordering.Infrastructure/DependencyInjection.cs`.
4. Register services by region:
   - Persistence
   - Domain Events
   - Factories
   - Specifications
5. Keep Infrastructure replaceable.
   - API should call `AddOrderingInfrastructure`.
   - Application should only know abstractions.

## API Layer

The API project is the composition root. It wires dependencies and exposes HTTP endpoints.

Main endpoint groups:

```http
POST   /customers
GET    /customers/{customerId}
PUT    /customers/{customerId}
POST   /customers/{customerId}/deactivate

POST   /products
GET    /products/{productId}
PUT    /products/{productId}
POST   /products/{productId}/mark-unavailable
POST   /products/{productId}/mark-available

POST   /orders
GET    /orders/{orderId}
POST   /orders/{orderId}/items
PATCH  /orders/{orderId}/items/{productId}
DELETE /orders/{orderId}/items/{productId}
POST   /orders/{orderId}/confirm
POST   /orders/{orderId}/cancel
```

The endpoint files call application services. They should not contain business rules.

### How to implement the API layer

1. Add endpoint files under `src/Ordering.Api/Endpoints`.
   - `CustomerEndpoints`
   - `ProductEndpoints`
   - `OrderEndpoints`
2. Register application services in `Program.cs`.
3. Call `AddOrderingInfrastructure`.
4. Map endpoint groups.
5. Add Swagger packages and configuration.
6. Update `launchSettings.json`.
   - Set `launchBrowser` to `true`.
   - Set `launchUrl` to `swagger`.

## Run the Project

Build:

```powershell
dotnet build OrderingSystem.slnx
```

Run tests:

```powershell
dotnet test OrderingSystem.slnx
```

Run the API:

```powershell
dotnet run --project src/Ordering.Api
```

Default local URLs:

```text
http://localhost:5210
https://localhost:7263
```

Health check:

```http
GET /health
```

## Swagger

Swagger UI is enabled in the Development environment.

Open:

```http
GET /swagger
```

Full local URL:

```text
http://localhost:5210/swagger
```

Swagger JSON:

```text
http://localhost:5210/swagger/v1/swagger.json
```

The launch profile is configured to open Swagger automatically when the API starts.

## Postman Scenario

Use this workflow after running the API locally.

### Environment Variables

Create a Postman environment with:

```text
baseUrl = http://localhost:5210
customerId =
productId =
orderId =
```

If you use HTTPS:

```text
baseUrl = https://localhost:7263
```

### 1. Health Check

```http
GET {{baseUrl}}/health
```

Expected response:

```json
{
  "status": "Healthy"
}
```

### 2. Add Customer

```http
POST {{baseUrl}}/customers
Content-Type: application/json

{
  "name": "Ada Lovelace",
  "email": "ada@example.com"
}
```

Save the returned `id` as `customerId`.

Example response:

```json
{
  "id": "copy-this-customer-id",
  "name": "Ada Lovelace",
  "email": "ada@example.com",
  "isActive": true,
  "createdAtUtc": "2026-09-15T00:00:00Z"
}
```

### 3. Modify Customer

```http
PUT {{baseUrl}}/customers/{{customerId}}
Content-Type: application/json

{
  "name": "Ada Byron",
  "email": "ada.byron@example.com"
}
```

The domain validates the name and email before changing either value.

### 4. Add Product

```http
POST {{baseUrl}}/products
Content-Type: application/json

{
  "name": "Laptop",
  "unitPrice": 250000
}
```

Save the returned `id` as `productId`.

Example response:

```json
{
  "id": "copy-this-product-id",
  "name": "Laptop",
  "unitPrice": 250000,
  "isAvailable": true,
  "createdAtUtc": "2026-09-15T00:00:00Z"
}
```

### 5. Modify Product

```http
PUT {{baseUrl}}/products/{{productId}}
Content-Type: application/json

{
  "name": "Developer Laptop",
  "unitPrice": 275000
}
```

### 6. Create Order

```http
POST {{baseUrl}}/orders
Content-Type: application/json

{
  "customerId": "{{customerId}}"
}
```

Save the returned `id` as `orderId`.

Example response:

```json
{
  "id": "copy-this-order-id",
  "customerId": "customer-id-from-step-2",
  "status": 1,
  "createdAtUtc": "2026-09-15T00:00:00Z",
  "totalAmount": 0,
  "items": []
}
```

### 7. Add Product to Order

```http
POST {{baseUrl}}/orders/{{orderId}}/items
Content-Type: application/json

{
  "productId": "{{productId}}",
  "quantity": 2
}
```

The order captures the product name and price at the time the item is added.

### 8. Get Order

```http
GET {{baseUrl}}/orders/{{orderId}}
```

Example response:

```json
{
  "id": "order-id-from-step-6",
  "customerId": "customer-id-from-step-2",
  "status": 1,
  "createdAtUtc": "2026-09-15T00:00:00Z",
  "totalAmount": 550000,
  "items": [
    {
      "id": "order-item-id",
      "productId": "product-id-from-step-4",
      "productName": "Developer Laptop",
      "unitPrice": 275000,
      "quantity": 2,
      "totalPrice": 550000
    }
  ]
}
```

### 9. Change Order Item Quantity

```http
PATCH {{baseUrl}}/orders/{{orderId}}/items/{{productId}}
Content-Type: application/json

{
  "quantity": 3
}
```

Expected total:

```text
825000
```

### 10. Remove Order Item

Use this before confirming the order:

```http
DELETE {{baseUrl}}/orders/{{orderId}}/items/{{productId}}
```

Add the item again before confirmation:

```http
POST {{baseUrl}}/orders/{{orderId}}/items
Content-Type: application/json

{
  "productId": "{{productId}}",
  "quantity": 1
}
```

### 11. Confirm Order

```http
POST {{baseUrl}}/orders/{{orderId}}/confirm
```

After confirmation, item changes are no longer allowed.

### 12. Try a Rule Violation

Try adding an item after confirmation:

```http
POST {{baseUrl}}/orders/{{orderId}}/items
Content-Type: application/json

{
  "productId": "{{productId}}",
  "quantity": 1
}
```

Expected response:

```json
{
  "error": "Only pending orders can be modified."
}
```

### 13. Cancel Order Alternative

Cancellation is only valid while an order is still pending. To test cancellation, create a new order and call:

```http
POST {{baseUrl}}/orders/{{orderId}}/cancel
```

## Testing

Domain tests verify business rules directly:

- Customer validation and inactive customer behavior.
- Product validation and availability behavior.
- Order lifecycle and item behavior.
- Value object validation and equality.
- Aggregate root architecture.
- Domain event recording.

Application tests verify use-case orchestration:

- Customer creation and update.
- Product creation and update.
- Order creation, item changes, confirmation, and missing entity behavior.
- Repository architecture rules.

### How to implement tests

1. Add Domain tests for:
   - value objects
   - aggregate behavior
   - domain events
2. Add Application tests for:
   - services
   - specifications
   - repository architecture
   - domain event dispatching
3. Use fake repositories in Application tests.
4. Use a fake domain event dispatcher in `OrderService` tests.
5. Run both validation commands:

   ```powershell
   dotnet test
   dotnet build
   ```

## Study Checklist

Use this checklist while reading the code:

- Start with `Ordering.Domain/Common`.
- Read `Ordering.Domain/Common/ValueObjects`.
- Read `Customer`, `Product`, and `Order`.
- Find where simple API values become value objects.
- Notice which classes are aggregate roots.
- Notice that `OrderItem` has no repository.
- Read `IOrderFactory` and `OrderFactory`.
- Read the order domain events and `AggregateRoot.DomainEvents`.
- Read application services after the domain model.
- Read `IDomainEventDispatcher` and the logging dispatcher.
- Read `ActiveCustomerSpecification` and `AvailableProductSpecification`.
- Read repository interfaces before repository implementations.
- Read API endpoints last.
- Run tests to see each rule expressed as executable examples.

## Useful Links

- [GitHub repository](https://github.com/PasinduUmayanga/domain-driven-design-sample)
- [AppVeyor project](https://ci.appveyor.com/project/PasinduUmayanga/domain-driven-design-sample)
- [AppVeyor build history](https://ci.appveyor.com/project/PasinduUmayanga/domain-driven-design-sample/history)
