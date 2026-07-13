using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence;

/// <summary>
/// Requisito (Desafio Fase 1 · RT-01/RT-02): "Entity Framework Core" para os modelos de usuários e
/// jogos, com "migrations para a criação do banco de dados".
/// </summary>
public sealed class CloudGamesDbContext : DbContext, IUnitOfWork
{
    public CloudGamesDbContext(DbContextOptions<CloudGamesDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Promotion> Promotions => Set<Promotion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CloudGamesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
        => base.SaveChangesAsync(cancellationToken);
}
