using PosTech.Fiap.CloudGames.Domain.Common;
using PosTech.Fiap.CloudGames.Domain.Events;
using PosTech.Fiap.CloudGames.Domain.Exceptions;

namespace PosTech.Fiap.CloudGames.Domain.Entities;

/// <summary>Raiz de agregado: promoção de desconto aplicável a um conjunto de jogos.</summary>
public sealed class Promotion : Entity
{
    private readonly List<PromotionGame> _games = new();

    public string Name { get; private set; } = null!;
    public decimal DiscountPercent { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<PromotionGame> Games => _games.AsReadOnly();

    // Construtor para o EF Core.
    private Promotion()
    {
    }

    public Promotion(string name, decimal discountPercent, DateTime startsAt, DateTime endsAt, IEnumerable<Guid> gameIds)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome da promoção é obrigatório.");

        if (discountPercent is <= 0 or > 100)
            throw new DomainException("O desconto deve estar entre 0 e 100%.");

        if (endsAt <= startsAt)
            throw new DomainException("A data de término deve ser posterior à data de início.");

        Name = name.Trim();
        DiscountPercent = discountPercent;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Active = true;
        CreatedAt = DateTime.UtcNow;

        foreach (var gameId in gameIds?.Distinct() ?? Enumerable.Empty<Guid>())
            _games.Add(new PromotionGame(Id, gameId));

        if (_games.Count == 0)
            throw new DomainException("A promoção deve conter ao menos um jogo.");

        Raise(new PromotionCreated(Id, Name));
    }

    public bool AppliesTo(Guid gameId) => _games.Any(g => g.GameId == gameId);

    public bool IsActiveOn(DateTime moment) => Active && moment >= StartsAt && moment <= EndsAt;

    public void Deactivate() => Active = false;
}
