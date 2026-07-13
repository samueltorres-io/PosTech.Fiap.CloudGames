using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Events;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using FluentAssertions;

namespace PosTech.Fiap.CloudGames.Domain.Tests.Entities;

public class PromotionTests
{
    private static Promotion NewPromotion(decimal discount = 20m, params Guid[] gameIds)
    {
        var ids = gameIds.Length == 0 ? new[] { Guid.NewGuid() } : gameIds;
        return new Promotion("Summer Sale", discount, new DateTime(2026, 1, 1), new DateTime(2026, 2, 1), ids);
    }

    [Fact]
    public void Constructor_WithValidData_ShouldRaisePromotionCreatedEvent()
    {
        var promotion = NewPromotion();

        promotion.DomainEvents.Should().ContainSingle(e => e is PromotionCreated);
        promotion.Games.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(120)]
    public void Constructor_WithInvalidDiscount_ShouldThrow(decimal discount)
    {
        var act = () => NewPromotion(discount);

        act.Should().Throw<DomainException>().WithMessage("*entre 0 e 100*");
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_ShouldThrow()
    {
        var act = () => new Promotion("X", 10m, new DateTime(2026, 2, 1), new DateTime(2026, 1, 1), new[] { Guid.NewGuid() });

        act.Should().Throw<DomainException>().WithMessage("*posterior*");
    }

    [Fact]
    public void Constructor_WithNoGames_ShouldThrow()
    {
        var act = () => new Promotion("X", 10m, new DateTime(2026, 1, 1), new DateTime(2026, 2, 1), Array.Empty<Guid>());

        act.Should().Throw<DomainException>().WithMessage("*ao menos um jogo*");
    }

    [Fact]
    public void IsActiveOn_ShouldRespectDateRangeAndFlag()
    {
        var promotion = NewPromotion();

        promotion.IsActiveOn(new DateTime(2026, 1, 15)).Should().BeTrue();
        promotion.IsActiveOn(new DateTime(2025, 12, 31)).Should().BeFalse();

        promotion.Deactivate();
        promotion.IsActiveOn(new DateTime(2026, 1, 15)).Should().BeFalse();
    }
}
