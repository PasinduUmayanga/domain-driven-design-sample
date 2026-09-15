namespace Ordering.Domain.Common.ValueObjects;

/// <summary>
/// Represents a non-negative money amount.
/// </summary>
public readonly record struct Money
{
    private Money(decimal amount)
    {
        Amount = amount;
    }

    public decimal Amount { get; }

    public static Money Create(decimal amount, string parameterName = "amount")
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Money amount cannot be negative.");
        }

        return new Money(amount);
    }

    public override string ToString() => Amount.ToString("0.##");

    public static implicit operator decimal(Money money) => money.Amount;
}
