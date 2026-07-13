using PosTech.Fiap.CloudGames.Domain.Entities;

namespace PosTech.Fiap.CloudGames.Domain.Repositories;

public interface IPromotionRepository
{
    Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Promotion?> GetActivePromotionForGameAsync(Guid gameId, DateTime moment, CancellationToken cancellationToken = default);

    Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default);

    void Update(Promotion promotion);

    void Remove(Promotion promotion);
}
