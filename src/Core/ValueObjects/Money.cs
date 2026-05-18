using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.ValueObjects;

// With this , Money = safe, validated, consistent financial object
// sealed  means cannot be inherited, that ensures immutability and prevents unintended modifications. 
//record for value-based equality, so two Money objects with same amount and currency are considered equal. 
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; } //eg USD, EUR, etc

    private Money() { } // for EF Core

    //Business Rules
    // Negative money rule and currency format check
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new DomainException("Currency Must be a 3-letter ISO Code..");

        Amount = Math.Round(amount, 2); // Round to 2 decimal places for cents
        Currency = currency.ToUpperInvariant(); //converts currency code to uppercase in a culture-safe way.eg "usd" becomes "USD".
    }

    //currency safety check, to prevent adding USD to EUR, etc.
    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot operate on {Currency}");
    }

    //currency addition and subtraction, with currency safety check
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) =>
       new(Amount * factor, Currency);

    //factory method for zero money, useful for initialization and default values.
    public static Money Zero(string currency) => new Money(0, currency);

    // override ToString for easy display, like "USD 100.00"
    public override string ToString() => $"{Currency} {Amount:F2}";
}