using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Repositories;

public sealed class GameRepository : IGameRepository
{
    private readonly CloudGamesDbContext _context;

    public GameRepository(CloudGamesDbContext context) => _context = context;

    public Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Games.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Games.AsNoTracking().OrderBy(g => g.Title).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Game>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        return await _context.Games.Where(g => idList.Contains(g.Id)).ToListAsync(cancellationToken);
    }

    public Task<bool> TitleExistsAsync(string title, CancellationToken cancellationToken = default)
        => _context.Games.AnyAsync(g => g.Title == title, cancellationToken);

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
        => await _context.Games.AddAsync(game, cancellationToken);

    public void Update(Game game) => _context.Games.Update(game);

    public void Remove(Game game) => _context.Games.Remove(game);
}
