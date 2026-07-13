using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;

namespace PosTech.Fiap.CloudGames.Application.Games;

public static class GameMappings
{
    public static GameResponse ToResponse(this Game game) => new(
        game.Id,
        game.Title,
        game.Description,
        game.Genre,
        game.Price.Amount,
        game.ReleaseDate,
        game.Active,
        game.CreatedAt);
}
