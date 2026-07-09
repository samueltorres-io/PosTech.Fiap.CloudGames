using FCG.Domain.Exceptions;
using FCG.Domain.ValueObjects;
using FluentAssertions;

namespace FCG.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldRoundToTwoDecimals()
    {
        var money = Money.Create(10.555m);

        money.Amount.Should().Be(10.56m);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrow()
    {
        var act = () => Money.Create(-1m);

        act.Should().Throw<DomainException>().WithMessage("*não pode ser negativo*");
    }

    [Theory]
    [InlineData(100, 10, 90)]
    [InlineData(100, 0, 100)]
    [InlineData(59.90, 50, 29.95)]
    public void ApplyDiscount_WithValidPercent_ShouldReduceAmount(decimal amount, decimal percent, decimal expected)
    {
        Money.Create(amount).ApplyDiscount(percent).Amount.Should().Be(expected);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(150)]
    public void ApplyDiscount_WithInvalidPercent_ShouldThrow(decimal percent)
    {
        var act = () => Money.Create(100m).ApplyDiscount(percent);

        act.Should().Throw<DomainException>().WithMessage("*entre 0 e 100*");
    }
}
