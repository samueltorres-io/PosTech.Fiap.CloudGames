using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Library;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace PosTech.Fiap.CloudGames.Application.Tests.Library;

public class LibraryServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IGameRepository> _games = new();
    private readonly Mock<IPromotionRepository> _promotions = new();
    private readonly Mock<IGameReadQueries> _read = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private LibraryService CreateSut() => new(_users.Object, _games.Object, _promotions.Object, _read.Object, _uow.Object);

    private static User NewUser() => new("Alice", Email.Create("alice@cloudgames.com"), "hash", UserRole.User);
    private static Game NewGame() => new("Half-Life", "FPS", "FPS", Money.Create(100m));

    [Fact]
    public async Task AcquireAsync_WithoutPromotion_ShouldChargeFullPrice()
    {
        var user = NewUser();
        var game = NewGame();
        _users.Setup(r => r.GetWithLibraryAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _games.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        _promotions.Setup(r => r.GetActivePromotionForGameAsync(game.Id, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotion?)null);
        var sut = CreateSut();

        var result = await sut.AcquireAsync(user.Id, game.Id);

        result.PricePaid.Should().Be(100m);
        result.DiscountApplied.Should().BeNull();
        user.OwnsGame(game.Id).Should().BeTrue();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcquireAsync_WithActivePromotion_ShouldApplyDiscount()
    {
        var user = NewUser();
        var game = NewGame();
        var promotion = new Promotion("Sale", 50m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new[] { game.Id });
        _users.Setup(r => r.GetWithLibraryAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _games.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        _promotions.Setup(r => r.GetActivePromotionForGameAsync(game.Id, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promotion);
        var sut = CreateSut();

        var result = await sut.AcquireAsync(user.Id, game.Id);

        result.PricePaid.Should().Be(50m);
        result.DiscountApplied.Should().Be(50m);
    }

    [Fact]
    public async Task AcquireAsync_WhenAlreadyOwned_ShouldThrowConflict()
    {
        var user = NewUser();
        var game = NewGame();
        user.AcquireGame(game, Money.Create(100m));
        _users.Setup(r => r.GetWithLibraryAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _games.Setup(r => r.GetByIdAsync(game.Id, It.IsAny<CancellationToken>())).ReturnsAsync(game);
        var sut = CreateSut();

        var act = () => sut.AcquireAsync(user.Id, game.Id);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task AcquireAsync_WhenGameMissing_ShouldThrowNotFound()
    {
        var user = NewUser();
        _users.Setup(r => r.GetWithLibraryAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _games.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Game?)null);
        var sut = CreateSut();

        var act = () => sut.AcquireAsync(user.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
