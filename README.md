# domain-driven-design-sample

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

## Step 3 - Create the Order Entity

Inside `src/Ordering.Domain`, create this structure:

```text
Ordering.Domain
|
+-- Orders
|   +-- Order.cs
|   +-- OrderStatus.cs
|
+-- Ordering.Domain.csproj
```

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
