using PosTech.Fiap.CloudGames.Domain.Exceptions;

namespace PosTech.Fiap.CloudGames.Domain.ValueObjects;

/// <summary>Valor monetário não-negativo, arredondado a 2 casas.</summary>
public sealed class Money
{
    public const string DefaultCurrency = "BRL";

    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero => new(0m, DefaultCurrency);

    public static Money Create(decimal amount, string currency = DefaultCurrency)
    {
        if (amount < 0)
            throw new DomainException("O valor monetário não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("A moeda é obrigatória.");

        return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), currency.ToUpperInvariant());
    }

    /// <summary>Aplica um desconto percentual (0–100) e devolve um novo valor.</summary>
    public Money ApplyDiscount(decimal percent)
    {
        if (percent is < 0 or > 100)
            throw new DomainException("O desconto deve estar entre 0 e 100%.");

        return Create(Amount * (1 - percent / 100m), Currency);
    }

    public override string ToString() => $"{Currency} {Amount:0.00}";

    public override bool Equals(object? obj)
        => obj is Money other && other.Amount == Amount && other.Currency == Currency;

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}
