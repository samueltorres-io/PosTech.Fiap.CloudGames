using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;

namespace PosTech.Fiap.CloudGames.Application.Games;

/// <summary>Casos de uso de escrita do catálogo de jogos (apenas administradores).</summary>
public sealed class GameCommandService
{
    private readonly IGameRepository _games;
    private readonly IUnitOfWork _unitOfWork;

    public GameCommandService(IGameRepository games, IUnitOfWork unitOfWork)
    {
        _games = games;
        _unitOfWork = unitOfWork;
    }

    public async Task<GameResponse> CreateAsync(CreateGameRequest request, CancellationToken cancellationToken = default)
    {
        if (await _games.TitleExistsAsync(request.Title, cancellationToken))
            throw new ConflictException($"Já existe um jogo com o título '{request.Title}'.");

        var game = new Game(
            request.Title,
            request.Description,
            request.Genre,
            Money.Create(request.Price),
            request.ReleaseDate);

        await _games.AddAsync(game, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return game.ToResponse();
    }

    public async Task<GameResponse> UpdateAsync(Guid id, UpdateGameRequest request, CancellationToken cancellationToken = default)
    {
        var game = await _games.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Jogo", id);

        game.Update(
            request.Title,
            request.Description,
            request.Genre,
            Money.Create(request.Price),
            request.ReleaseDate);

        _games.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return game.ToResponse();
    }

    /// <summary>Desativa o jogo (soft delete), preservando o histórico das bibliotecas.</summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await _games.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Jogo", id);

        game.Deactivate();
        _games.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
