using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence;

/// <summary>Popula dados iniciais: um administrador padrão e alguns jogos de exemplo.</summary>
public sealed class DatabaseSeeder
{
    public const string DefaultAdminEmail = "admin@cloudgames.com";
    public const string DefaultAdminPassword = "Admin@123";

    private readonly CloudGamesDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(CloudGamesDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAdminAsync(cancellationToken);
        await SeedGamesAsync(cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
    {
        var adminEmail = Email.Create(DefaultAdminEmail);
        if (await _context.Users.AnyAsync(u => u.Email == adminEmail, cancellationToken))
            return;

        var admin = new User(
            "Administrador FCG",
            adminEmail,
            _passwordHasher.Hash(DefaultAdminPassword),
            UserRole.Administrator);

        await _context.Users.AddAsync(admin, cancellationToken);
    }

    private async Task SeedGamesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Games.AnyAsync(cancellationToken))
            return;

        var games = new[]
        {
            new Game("The Witcher 3: Wild Hunt", "RPG de mundo aberto", "RPG", Money.Create(79.90m), new DateOnly(2015, 5, 19)),
            new Game("Hades", "Roguelike de ação", "Ação", Money.Create(49.90m), new DateOnly(2020, 9, 17)),
            new Game("Stardew Valley", "Simulador de fazenda", "Simulação", Money.Create(24.90m), new DateOnly(2016, 2, 26)),
            new Game("Celeste", "Plataforma desafiador", "Plataforma", Money.Create(37.90m), new DateOnly(2018, 1, 25))
        };

        await _context.Games.AddRangeAsync(games, cancellationToken);
    }
}
