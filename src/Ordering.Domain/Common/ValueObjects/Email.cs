using System.Net.Mail;

namespace Ordering.Domain.Common.ValueObjects;

/// <summary>
/// Represents a validated customer email address.
/// </summary>
public readonly record struct Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Customer email cannot be empty.", nameof(value));
        }

        var trimmedValue = value.Trim();

        if (!IsValid(trimmedValue))
        {
            throw new ArgumentException("Customer email must be valid.", nameof(value));
        }

        return new Email(trimmedValue);
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    private static bool IsValid(string value)
    {
        try
        {
            var mailAddress = new MailAddress(value);

            return string.Equals(
                mailAddress.Address,
                value,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
