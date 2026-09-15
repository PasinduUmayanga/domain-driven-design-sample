using Ordering.Domain.Common.ValueObjects;

namespace Ordering.Domain.Products;

/// <summary>
/// Represents a product that can be added to an order.
/// </summary>
public sealed class Product : Ordering.Domain.Common.AggregateRoot
{
    private ProductName _name;

    private Money _unitPrice;

    public string Name => _name.Value;

    public decimal UnitPrice => _unitPrice.Amount;

    public bool IsAvailable { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Product()
    {
    }

    private Product(Guid id, string name, decimal unitPrice)
    {
        Id = id;
        _name = ProductName.Create(name);
        _unitPrice = Money.Create(unitPrice, nameof(unitPrice));
        IsAvailable = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Product Create(string name, decimal unitPrice)
    {
        var productName = ProductName.Create(name);
        var price = Money.Create(unitPrice, nameof(unitPrice));

        return new Product(Guid.NewGuid(), productName.Value, price.Amount);
    }

    public void Rename(string name)
    {
        _name = ProductName.Create(name);
    }

    public void ChangePrice(decimal unitPrice)
    {
        _unitPrice = Money.Create(unitPrice, nameof(unitPrice));
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
}
