using FCG.Application.Games.Dtos;

namespace FCG.Application.Abstractions;

/// <summary>
/// Consultas de leitura de alta performance (implementadas com Dapper na infraestrutura).
/// Usadas no catálogo e na biblioteca, evitando o overhead do change-tracking do EF.
/// </summary>
public interface IGameReadQueries
{
    Task<IReadOnlyList<GameCatalogItem>> SearchCatalogAsync(GameFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LibraryItem>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default);
}
