using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Repositories;

public sealed class PromotionRepository : IPromotionRepository
{
    private readonly CloudGamesDbContext _context;

    public PromotionRepository(CloudGamesDbContext context) => _context = context;

    public Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Promotions.Include(p => p.Games).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Promotion>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Promotions
            .AsNoTracking()
            .Include(p => p.Games)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Promotion?> GetActivePromotionForGameAsync(Guid gameId, DateTime moment, CancellationToken cancellationToken = default)
        => _context.Promotions
            .Where(p => p.Active
                        && p.StartsAt <= moment
                        && p.EndsAt >= moment
                        && p.Games.Any(g => g.GameId == gameId))
            .OrderByDescending(p => p.DiscountPercent)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
        => await _context.Promotions.AddAsync(promotion, cancellationToken);

    public void Update(Promotion promotion) => _context.Promotions.Update(promotion);

    public void Remove(Promotion promotion) => _context.Promotions.Remove(promotion);
}
