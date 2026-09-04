# domain-driven-design-sample
<img width="2048" height="768" alt="image" src="https://github.com/user-attachments/assets/f97b2c1e-1f22-4d0c-8358-d4f1fdf0c23c" />

# Step 1 — What is Domain-Driven Design?

Before coding, understand this idea.

DDD means:

Build your software around the business domain and business rules instead of around database tables.

For example

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/fead04cd-8239-4845-8a0b-96dda3f9de22" />


## What is a Domain?

The domain is the business problem that the software is solving.

For our example:

Domain: Online Ordering

Inside that domain we have concepts such as:

<img width="1536" height="1024" alt="image" src="https://github.com/user-attachments/assets/9e3b50a8-eb71-434d-b63c-5dc981704fdf" />

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
