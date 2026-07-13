using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using PosTech.Fiap.CloudGames.Domain.Repositories;

namespace PosTech.Fiap.CloudGames.Application.Games;

/// <summary>Casos de uso de leitura do catálogo (Dapper para consultas, EF para detalhe).</summary>
public sealed class GameQueryService
{
    private readonly IGameReadQueries _readQueries;
    private readonly IGameRepository _games;

    public GameQueryService(IGameReadQueries readQueries, IGameRepository games)
    {
        _readQueries = readQueries;
        _games = games;
    }

    public Task<IReadOnlyList<GameCatalogItem>> SearchAsync(GameFilter filter, CancellationToken cancellationToken = default)
        => _readQueries.SearchCatalogAsync(Normalize(filter), cancellationToken);

    public async Task<GameResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await _games.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Jogo", id);

        return game.ToResponse();
    }

    private static GameFilter Normalize(GameFilter filter) => filter with
    {
        Page = filter.Page < 1 ? 1 : filter.Page,
        PageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize
    };
}
