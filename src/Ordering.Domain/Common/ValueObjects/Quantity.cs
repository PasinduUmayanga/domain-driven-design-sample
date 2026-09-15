namespace Ordering.Domain.Common.ValueObjects;

/// <summary>
/// Represents a positive item quantity.
/// </summary>
public readonly record struct Quantity
{
    private Quantity(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Quantity Create(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Quantity must be greater than zero.");
        }

        return new Quantity(value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(Quantity quantity) => quantity.Value;
}
