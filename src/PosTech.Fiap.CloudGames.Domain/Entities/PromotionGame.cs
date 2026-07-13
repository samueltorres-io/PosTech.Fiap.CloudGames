namespace PosTech.Fiap.CloudGames.Domain.Entities;

/// <summary>Associação entre uma promoção e um jogo participante.</summary>
public sealed class PromotionGame
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PromotionId { get; private set; }
    public Guid GameId { get; private set; }

    // Construtor para o EF Core.
    private PromotionGame()
    {
    }

    public PromotionGame(Guid promotionId, Guid gameId)
    {
        PromotionId = promotionId;
        GameId = gameId;
    }
}
