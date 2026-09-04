# Step 1 — What is Domain-Driven Design?

Before coding, understand this idea.

DDD means:

Build your software around the business domain and business rules instead of around database tables.

For example, without DDD, someone might start the ordering system like this:

```text
Database
   ↓
Orders table
OrderItems table
Products table
   ↓
EF Entities
   ↓
Controllers
```

The database becomes the center of the design.

DDD thinks differently:

```text
Business
   ↓
Order
OrderItem
Customer
Product
Payment
   ↓
Business Rules
   ↓
Application
   ↓
Infrastructure
   ↓
Database
```

The business model is the center.

## What is a Domain?

The domain is the business problem that the software is solving.

For our example:

Domain: Online Ordering

Inside that domain we have concepts such as:

```text
Customer
Order
Order Item
Product
Payment
Shipment
Discount
```

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
