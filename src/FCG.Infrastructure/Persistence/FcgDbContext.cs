using FCG.Domain.Entities;
using FCG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Persistence;

/// <summary>
/// Requisito (Desafio Fase 1 · RT-01/RT-02): "Entity Framework Core" para os modelos de usuários e
/// jogos, com "migrations para a criação do banco de dados".
/// </summary>
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
