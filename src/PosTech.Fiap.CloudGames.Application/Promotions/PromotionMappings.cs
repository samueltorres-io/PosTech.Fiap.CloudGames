using PosTech.Fiap.CloudGames.Application.Promotions.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;

namespace PosTech.Fiap.CloudGames.Application.Promotions;

public static class PromotionMappings
{
    public static PromotionResponse ToResponse(this Promotion promotion) => new(
        promotion.Id,
        promotion.Name,
        promotion.DiscountPercent,
        promotion.StartsAt,
        promotion.EndsAt,
        promotion.Active,
        promotion.Games.Select(g => g.GameId).ToList());
}
