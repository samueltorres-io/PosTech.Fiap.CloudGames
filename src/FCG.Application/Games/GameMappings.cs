using FCG.Application.Games.Dtos;
using FCG.Domain.Entities;

namespace FCG.Application.Games;

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
