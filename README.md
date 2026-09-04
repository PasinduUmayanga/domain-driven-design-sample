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

```text
OrderingSystem
|
+-- src
|   +-- Ordering.Domain
|   +-- Ordering.Application
|   +-- Ordering.Infrastructure
|   +-- Ordering.Api
|
+-- tests
    +-- Ordering.Domain.Tests
    +-- Ordering.Application.Tests
```

The dependency direction will eventually be:

```text
               Ordering.Api
                    |
                    v
          Ordering.Application
                    |
                    v
             Ordering.Domain


          Ordering.Infrastructure
                    |
                    +------------> Application
                    |
                    +------------> Domain
```

## Create the Solution in Visual Studio

You can create this solution using the Visual Studio user interface.

Open Visual Studio and select **Create a new project**.

Screenshot to add: `docs/images/visual-studio-create-new-project.png`

Search for **Blank Solution**, select it, and click **Next**.

Set the solution name to:

```text
OrderingSystem
```

Choose the location where you want to save the project, then click **Create**.

Screenshot to add: `docs/images/visual-studio-blank-solution.png`

After the solution is created, add two solution folders:

1. Right-click the solution.
2. Select **Add**.
3. Select **New Solution Folder**.
4. Name the first folder `src`.
5. Repeat the same steps and name the second folder `tests`.

Screenshot to add: `docs/images/visual-studio-add-solution-folders.png`

Now add the Domain project:

1. Right-click the `src` solution folder.
2. Select **Add**.
3. Select **New Project**.
4. Search for **Class Library**.
5. Select the C# Class Library template and click **Next**.
6. Set the project name to `Ordering.Domain`.
7. Choose `.NET 10.0` as the target framework.
8. Click **Create**.

Add the Application project the same way:

```text
Project name: Ordering.Application
Template: Class Library
Target framework: .NET 10.0
Solution folder: src
```

Add the Infrastructure project the same way:

```text
Project name: Ordering.Infrastructure
Template: Class Library
Target framework: .NET 10.0
Solution folder: src
```

Add the API project:

```text
Project name: Ordering.Api
Template: ASP.NET Core Web API
Target framework: .NET 10.0
Solution folder: src
```

Screenshot to add: `docs/images/visual-studio-add-projects.png`

Now add the test projects under the `tests` solution folder.

Add the Domain test project:

```text
Project name: Ordering.Domain.Tests
Template: xUnit Test Project
Target framework: .NET 10.0
Solution folder: tests
```

Add the Application test project:

```text
Project name: Ordering.Application.Tests
Template: xUnit Test Project
Target framework: .NET 10.0
Solution folder: tests
```

Screenshot to add: `docs/images/visual-studio-add-test-projects.png`

When finished, the solution should look like this:

```text
OrderingSystem
|
+-- src
|   +-- Ordering.Domain
|   +-- Ordering.Application
|   +-- Ordering.Infrastructure
|   +-- Ordering.Api
|
+-- tests
    +-- Ordering.Domain.Tests
    +-- Ordering.Application.Tests
```

Screenshot to add: `docs/images/visual-studio-final-solution-structure.png`
