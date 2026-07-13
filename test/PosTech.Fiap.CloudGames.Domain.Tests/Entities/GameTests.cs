using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Events;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentAssertions;

namespace PosTech.Fiap.CloudGames.Domain.Tests.Entities;

public class GameTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldRaiseGameCreatedEvent()
    {
        var game = new Game("Portal 2", "Puzzle", "Puzzle", Money.Create(39.90m));

        game.DomainEvents.Should().ContainSingle(e => e is GameCreated);
        game.Active.Should().BeTrue();
        game.Title.Should().Be("Portal 2");
    }

    [Theory]
    [InlineData("", "Puzzle", "título")]
    [InlineData("Portal 2", "", "gênero")]
    public void Constructor_WithMissingRequiredField_ShouldThrow(string title, string genre, string expected)
    {
        var act = () => new Game(title, "desc", genre, Money.Create(10m));

        act.Should().Throw<DomainException>().WithMessage($"*{expected}*");
    }

    [Fact]
    public void Update_ShouldChangeMutableData()
    {
        var game = new Game("Portal", "Puzzle", "Puzzle", Money.Create(19.90m));

        game.Update("Portal 2", "Sequel", "Puzzle-Platformer", Money.Create(39.90m), new DateOnly(2011, 4, 18));

        game.Title.Should().Be("Portal 2");
        game.Genre.Should().Be("Puzzle-Platformer");
        game.Price.Amount.Should().Be(39.90m);
        game.ReleaseDate.Should().Be(new DateOnly(2011, 4, 18));
    }

    [Fact]
    public void Deactivate_ShouldMarkAsInactive()
    {
        var game = new Game("Portal", "Puzzle", "Puzzle", Money.Create(19.90m));

        game.Deactivate();

        game.Active.Should().BeFalse();
    }
}
