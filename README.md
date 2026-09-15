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
- [Implementation Steps](#implementation-steps)
- [Solution Structure](#solution-structure)
- [Architecture Rules](#architecture-rules)
- [Domain Model](#domain-model)
- [Value Objects](#value-objects)
- [Aggregate Roots](#aggregate-roots)
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

## Implementation Steps

Use these steps to study or rebuild the sample in a clean project.

1. **Create the Solution Structure**

   Create four source projects and two test projects:

   ```text
   src/Ordering.Domain
   src/Ordering.Application
   src/Ordering.Infrastructure
   src/Ordering.Api
   tests/Ordering.Domain.Tests
   tests/Ordering.Application.Tests
   ```

   Keep dependencies pointing inward: API and Infrastructure depend on Application, Application depends on Domain, and Domain depends on no other project.

2. **Add Domain Building Blocks**

   Create common domain base types:

   - `Entity`
   - `AggregateRoot`
   - `IDomainEvent`

   These types introduce identity, aggregate boundaries, and pending domain events.

3. **Add Value Objects**

   Add value objects before writing rich entities so small validation rules have a clear home:

   - `Email`
   - `Money`
   - `ProductName`
   - `Quantity`

   Use value objects inside aggregates, but keep API DTOs simple with strings, decimals, and integers.

4. **Add Aggregate Roots and Entities**

   Create the core domain model:

   - `Customer : AggregateRoot`
   - `Product : AggregateRoot`
   - `Order : AggregateRoot`
   - `OrderItem : Entity`

   Keep `OrderItem` inside the `Order` aggregate. Do not create an `OrderItem` repository.

5. **Add Domain Events**

   Let aggregate roots record events after successful state changes:

   - `OrderCreatedDomainEvent`
   - `OrderItemAddedDomainEvent`
   - `OrderConfirmedDomainEvent`
   - `OrderCancelledDomainEvent`

   Record events inside the aggregate, but do not dispatch them from the Domain layer.

6. **Add Application Use Cases**

   Create application services and repository interfaces:

   - `CustomerService`
   - `ProductService`
   - `OrderService`
   - `ICustomerRepository`
   - `IProductRepository`
   - `IOrderRepository`

   Application services load aggregates, call domain behavior, save aggregates, and return response DTOs.

7. **Add Domain Event Dispatching**

   Add `IDomainEventDispatcher` in the Application layer.

   After persistence succeeds, dispatch pending aggregate events and clear them:

   ```csharp
   await orderRepository.SaveAsync(order, cancellationToken);
   await domainEventDispatcher.DispatchAsync(order.DomainEvents.ToArray(), cancellationToken);
   order.ClearDomainEvents();
   ```

   Infrastructure provides `LoggingDomainEventDispatcher` for this sample.

8. **Add Specification Pattern**

   Create named specifications for cross-aggregate rules:

   - `ActiveCustomerSpecification`
   - `AvailableProductSpecification`

   Use them in `OrderService` for rules like:

   - active customers can place orders
   - available products can be added to orders

   Keep aggregate invariants, such as pending-order checks, inside the aggregate itself.

9. **Add Infrastructure Adapters**

   Implement in-memory repositories in Infrastructure:

   - `InMemoryCustomerRepository`
   - `InMemoryProductRepository`
   - `InMemoryOrderRepository`

   Register repositories, domain event dispatcher, and specifications in `AddOrderingInfrastructure`.

10. **Add API Endpoints and Swagger**

    Expose the use cases through minimal API endpoint groups:

    - customers
    - products
    - orders

    Enable Swagger so the workflow can be tested from the browser.

11. **Add Tests**

    Add focused tests for:

    - value object validation
    - aggregate behavior
    - domain events
    - specifications
    - application service orchestration
    - repository architecture rules

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
|   |-- Order.cs
|   |-- OrderItem.cs
|   `-- OrderStatus.cs
`-- Products
    `-- Product.cs
```

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

Run:

```powershell
dotnet test
```

## Study Checklist

Use this checklist while reading the code:

- Start with `Ordering.Domain/Common`.
- Read `Ordering.Domain/Common/ValueObjects`.
- Read `Customer`, `Product`, and `Order`.
- Find where simple API values become value objects.
- Notice which classes are aggregate roots.
- Notice that `OrderItem` has no repository.
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
