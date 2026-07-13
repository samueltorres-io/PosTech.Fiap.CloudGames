using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly CloudGamesDbContext _context;

    public UserRepository(CloudGamesDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = Domain.ValueObjects.Email.Create(email);
        return _context.Users.FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
    }

    public Task<User?> GetWithLibraryAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Users.Include(u => u.Library).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = Domain.ValueObjects.Email.Create(email);
        return _context.Users.AnyAsync(u => u.Email == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(user, cancellationToken);

    public void Update(User user) => _context.Users.Update(user);

    public void Remove(User user) => _context.Users.Remove(user);
}
