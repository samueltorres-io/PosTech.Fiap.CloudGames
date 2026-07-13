using PosTech.Fiap.CloudGames.Domain.Entities;

namespace PosTech.Fiap.CloudGames.Domain.Repositories;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Game>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default);

    Task AddAsync(Game game, CancellationToken cancellationToken = default);

    void Update(Game game);

    void Remove(Game game);
}
