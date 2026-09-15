using EmailAddress = Ordering.Domain.Common.ValueObjects.Email;

namespace Ordering.Domain.Customers;

/// <summary>
/// Represents a customer who can place orders.
/// </summary>
public sealed class Customer : Ordering.Domain.Common.AggregateRoot
{
    private EmailAddress _email;

    public string Name { get; private set; } = string.Empty;

    public string Email => _email.Value;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Customer()
    {
    }

    private Customer(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        _email = EmailAddress.Create(email);
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Customer Register(string name, string email)
    {
        ValidateName(name);
        var customerEmail = EmailAddress.Create(email);

        return new Customer(Guid.NewGuid(), name.Trim(), customerEmail.Value);
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

        _email = EmailAddress.Create(email);
    }

    public void UpdateProfile(string name, string email)
    {
        EnsureActive();
        ValidateName(name);
        var customerEmail = EmailAddress.Create(email);

        Name = name.Trim();
        _email = customerEmail;
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
}
