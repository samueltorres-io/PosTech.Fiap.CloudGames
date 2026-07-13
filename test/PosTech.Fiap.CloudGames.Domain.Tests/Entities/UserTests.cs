using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Domain.Events;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentAssertions;

namespace PosTech.Fiap.CloudGames.Domain.Tests.Entities;

public class UserTests
{
    private static User NewUser(UserRole role = UserRole.User)
        => new("Alice", Email.Create("alice@cloudgames.com"), "hash", role);

    private static Game NewGame(string title = "Half-Life")
        => new(title, "Sci-fi FPS", "FPS", Money.Create(59.90m));

    [Fact]
    public void Constructor_ShouldRaiseUserRegisteredEvent()
    {
        var user = NewUser();

        user.DomainEvents.Should().ContainSingle(e => e is UserRegistered);
        user.Active.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        var act = () => new User("  ", Email.Create("a@b.com"), "hash");

        act.Should().Throw<DomainException>().WithMessage("*nome*");
    }

    [Fact]
    public void AcquireGame_ShouldAddToLibraryAndRaiseEvent()
    {
        var user = NewUser();
        var game = NewGame();

        user.AcquireGame(game, Money.Create(59.90m));

        user.Library.Should().ContainSingle(g => g.GameId == game.Id);
        user.OwnsGame(game.Id).Should().BeTrue();
        user.DomainEvents.Should().Contain(e => e is GameAcquired);
    }

    [Fact]
    public void AcquireGame_WhenAlreadyOwned_ShouldThrow()
    {
        var user = NewUser();
        var game = NewGame();
        user.AcquireGame(game, Money.Create(59.90m));

        var act = () => user.AcquireGame(game, Money.Create(59.90m));

        act.Should().Throw<DomainException>().WithMessage("*já possui*");
    }

    [Fact]
    public void IsAdministrator_ShouldReflectRole()
    {
        NewUser(UserRole.Administrator).IsAdministrator.Should().BeTrue();
        NewUser(UserRole.User).IsAdministrator.Should().BeFalse();
    }
}
