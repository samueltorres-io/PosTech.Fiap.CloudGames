using PosTech.Fiap.CloudGames.Domain.ValueObjects;

namespace PosTech.Fiap.CloudGames.Domain.Entities;

/// <summary>Item da biblioteca: um jogo adquirido por um usuário.</summary>
public sealed class UserGame
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal PricePaid { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    // Construtor para o EF Core.
    private UserGame()
    {
    }

    public UserGame(Guid userId, Guid gameId, Money pricePaid)
    {
        UserId = userId;
        GameId = gameId;
        PricePaid = pricePaid.Amount;
        AcquiredAt = DateTime.UtcNow;
    }
}
