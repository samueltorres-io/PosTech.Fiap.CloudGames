namespace PosTech.Fiap.CloudGames.Application.Promotions.Dtos;

/// <summary>Dados para criar uma promoção.</summary>
public sealed record CreatePromotionRequest(
    string Name,
    decimal DiscountPercent,
    DateTime StartsAt,
    DateTime EndsAt,
    IReadOnlyList<Guid> GameIds);

/// <summary>Representação de uma promoção retornada pela API.</summary>
public sealed record PromotionResponse(
    Guid Id,
    string Name,
    decimal DiscountPercent,
    DateTime StartsAt,
    DateTime EndsAt,
    bool Active,
    IReadOnlyList<Guid> GameIds);
