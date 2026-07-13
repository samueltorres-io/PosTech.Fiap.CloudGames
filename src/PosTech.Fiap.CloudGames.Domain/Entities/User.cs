using PosTech.Fiap.CloudGames.Domain.Common;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Domain.Events;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;

namespace PosTech.Fiap.CloudGames.Domain.Entities;

/// <summary>
/// Requisito (Desafio Fase 1 · RF-01): "identificação do cliente por nome, e-mail e senha".
/// Raiz de agregado: usuário da plataforma e sua biblioteca de jogos.
/// </summary>
public sealed class User : Entity
{
    private readonly List<UserGame> _library = new();

    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<UserGame> Library => _library.AsReadOnly();

    // Construtor para o EF Core.
    private User()
    {
    }

    public User(string name, Email email, string passwordHash, UserRole role = UserRole.User)
    {
        Rename(name);
        Email = email ?? throw new DomainException("O e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("O hash da senha é obrigatório.");

        PasswordHash = passwordHash;
        Role = role;
        Active = true;
        CreatedAt = DateTime.UtcNow;

        Raise(new UserRegistered(Id, Email.Value));
    }

    public bool IsAdministrator => Role == UserRole.Administrator;

    public bool OwnsGame(Guid gameId) => _library.Any(g => g.GameId == gameId);

    /// <summary>Adiciona um jogo à biblioteca, impedindo duplicatas.</summary>
    public UserGame AcquireGame(Game game, Money pricePaid)
    {
        ArgumentNullException.ThrowIfNull(game);

        if (OwnsGame(game.Id))
            throw new DomainException("O usuário já possui este jogo na biblioteca.");

        var item = new UserGame(Id, game.Id, pricePaid);
        _library.Add(item);
        Raise(new GameAcquired(Id, game.Id));
        return item;
    }

    public void ChangeRole(UserRole role) => Role = role;

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("O hash da senha é obrigatório.");

        PasswordHash = passwordHash;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome é obrigatório.");

        Name = name.Trim();
    }

    public void Activate() => Active = true;

    public void Deactivate() => Active = false;
}
