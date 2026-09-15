namespace Ordering.Domain.Products;

/// <summary>
/// Represents a product that can be added to an order.
/// </summary>
public sealed class Product : Ordering.Domain.Common.AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public bool IsAvailable { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Product()
    {
    }

    private Product(Guid id, string name, decimal unitPrice)
    {
        Id = id;
        Name = name;
        UnitPrice = unitPrice;
        IsAvailable = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Product Create(string name, decimal unitPrice)
    {
        ValidateName(name);
        ValidateUnitPrice(unitPrice);

        return new Product(Guid.NewGuid(), name.Trim(), unitPrice);
    }

    public void Rename(string name)
    {
        ValidateName(name);

        Name = name.Trim();
    }

    public void ChangePrice(decimal unitPrice)
    {
        ValidateUnitPrice(unitPrice);

        UnitPrice = unitPrice;
    }

    public void MarkUnavailable()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("Product is already unavailable.");
        }

        IsAvailable = false;
    }

    public void MarkAvailable()
    {
        if (IsAvailable)
        {
            throw new InvalidOperationException("Product is already available.");
        }

        IsAvailable = true;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        }
    }

    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Product price cannot be negative.");
        }
    }
}
