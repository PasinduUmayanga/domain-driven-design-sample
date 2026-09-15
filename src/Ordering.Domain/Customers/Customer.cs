namespace Ordering.Domain.Customers;

/// <summary>
/// Represents a customer who can place orders.
/// </summary>
public sealed class Customer : Ordering.Domain.Common.AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Customer()
    {
    }

    private Customer(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Customer Register(string name, string email)
    {
        ValidateName(name);
        ValidateEmail(email);

        return new Customer(Guid.NewGuid(), name.Trim(), email.Trim());
    }

    public void ChangeName(string name)
    {
        EnsureActive();
        ValidateName(name);

        Name = name.Trim();
    }

    public void ChangeEmail(string email)
    {
        EnsureActive();
        ValidateEmail(email);

        Email = email.Trim();
    }

    public void Deactivate()
    {
        EnsureActive();

        IsActive = false;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Inactive customers cannot be modified.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name cannot be empty.", nameof(name));
        }
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Customer email cannot be empty.", nameof(email));
        }

        if (!email.Contains('@', StringComparison.Ordinal))
        {
            throw new ArgumentException("Customer email must be valid.", nameof(email));
        }
    }
}
