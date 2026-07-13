using PosTech.Fiap.CloudGames.Infrastructure.Persistence;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace PosTech.Fiap.CloudGames.Api.GraphQL;

/// <summary>
/// Requisito (Desafio Fase 1 · RT-08, opcional): "utilizar GraphQL para consulta avançada de jogos,
/// permitindo filtragens dinâmicas".
/// </summary>
public sealed class Query
{
    /// <summary>Catálogo de jogos ativos com filtragem e ordenação dinâmicas.</summary>
    [GraphQLDescription("Consulta o catálogo de jogos ativos, permitindo filtragem e ordenação dinâmicas.")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<GameGraphDto>> GetGames(
        [Service] CloudGamesDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var games = await dbContext.Games
            .AsNoTracking()
            .Where(g => g.Active)
            .ToListAsync(cancellationToken);

        return games.Select(g => new GameGraphDto
        {
            Id = g.Id,
            Title = g.Title,
            Description = g.Description,
            Genre = g.Genre,
            Price = g.Price.Amount,
            ReleaseDate = g.ReleaseDate
        });
    }
}
