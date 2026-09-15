namespace Ordering.Domain.Common.ValueObjects;

/// <summary>
/// Represents a non-empty product name.
/// </summary>
public readonly record struct ProductName
{
    private ProductName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ProductName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Product name cannot be empty.", nameof(value));
        }

        return new ProductName(value.Trim());
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductName productName) => productName.Value;
}
