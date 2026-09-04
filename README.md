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
