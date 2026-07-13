using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Games;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace PosTech.Fiap.CloudGames.Application.Tests.Games;

public class GameCommandServiceTests
{
    private readonly Mock<IGameRepository> _games = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private GameCommandService CreateSut() => new(_games.Object, _uow.Object);

    [Fact]
    public async Task CreateAsync_WithUniqueTitle_ShouldPersistGame()
    {
        _games.Setup(r => r.TitleExistsAsync("Portal 2", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var sut = CreateSut();

        var response = await sut.CreateAsync(new CreateGameRequest("Portal 2", "Puzzle", "Puzzle", 39.90m, null));

        response.Title.Should().Be("Portal 2");
        response.Price.Should().Be(39.90m);
        _games.Verify(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateTitle_ShouldThrowConflict()
    {
        _games.Setup(r => r.TitleExistsAsync("Portal 2", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var sut = CreateSut();

        var act = () => sut.CreateAsync(new CreateGameRequest("Portal 2", "Puzzle", "Puzzle", 39.90m, null));

        await act.Should().ThrowAsync<ConflictException>();
        _games.Verify(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenGameMissing_ShouldThrowNotFound()
    {
        _games.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Game?)null);
        var sut = CreateSut();

        var act = () => sut.UpdateAsync(Guid.NewGuid(), new UpdateGameRequest("X", null, "Y", 10m, null));

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
