using FCG.Domain.Entities;
using FCG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence;

public sealed class FcgDbContext : DbContext, IUnitOfWork
{
    public FcgDbContext(DbContextOptions<FcgDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Promotion> Promotions => Set<Promotion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
        => base.SaveChangesAsync(cancellationToken);
}
