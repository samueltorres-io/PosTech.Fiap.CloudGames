using Dapper;
using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Dapper;

/// <summary>
/// Requisito (Desafio Fase 1 · RT-04, opcional): "utilizar Dapper para consultas de alta performance".
/// Consultas de leitura (catálogo com preço promocional efetivo e biblioteca do usuário).
/// </summary>
public sealed class GameReadQueries : IGameReadQueries
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GameReadQueries(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<GameCatalogItem>> SearchCatalogAsync(GameFilter filter, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT  g.id                              AS Id,
                    g.title                           AS Title,
                    g.description                     AS Description,
                    g.genre                           AS Genre,
                    g.price                           AS Price,
                    promo.discount_percent            AS DiscountPercent,
                    CASE WHEN promo.discount_percent IS NOT NULL
                         THEN ROUND(g.price * (1 - promo.discount_percent / 100.0), 2)
                         ELSE g.price END             AS EffectivePrice,
                    g.release_date                    AS ReleaseDate
            FROM games g
            LEFT JOIN LATERAL (
                SELECT pr.discount_percent
                FROM promotions pr
                JOIN promotion_games pg ON pg.promotion_id = pr.id
                WHERE pg.game_id = g.id
                  AND pr.active = TRUE
                  AND now() BETWEEN pr.starts_at AND pr.ends_at
                ORDER BY pr.discount_percent DESC
                LIMIT 1
            ) promo ON TRUE
            WHERE g.active = TRUE
              AND (@Title    IS NULL OR g.title ILIKE '%' || @Title || '%')
              AND (@Genre    IS NULL OR g.genre = @Genre)
              AND (@MinPrice IS NULL OR g.price >= @MinPrice)
              AND (@MaxPrice IS NULL OR g.price <= @MaxPrice)
            ORDER BY g.title
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            filter.Title,
            filter.Genre,
            filter.MinPrice,
            filter.MaxPrice,
            filter.PageSize,
            Offset = (filter.Page - 1) * filter.PageSize
        };

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<GameCatalogItem>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return result.ToList();
    }

    public async Task<IReadOnlyList<LibraryItem>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT  ug.game_id     AS GameId,
                    g.title        AS Title,
                    g.genre        AS Genre,
                    ug.price_paid  AS PricePaid,
                    ug.acquired_at AS AcquiredAt
            FROM user_games ug
            JOIN games g ON g.id = ug.game_id
            WHERE ug.user_id = @UserId
            ORDER BY ug.acquired_at DESC;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<LibraryItem>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
        return result.ToList();
    }
}
