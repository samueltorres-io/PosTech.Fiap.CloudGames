using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Promotions.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Repositories;

namespace PosTech.Fiap.CloudGames.Application.Promotions;

/// <summary>Casos de uso de promoções (apenas administradores).</summary>
public sealed class PromotionService
{
    private readonly IPromotionRepository _promotions;
    private readonly IGameRepository _games;
    private readonly IUnitOfWork _unitOfWork;

    public PromotionService(IPromotionRepository promotions, IGameRepository games, IUnitOfWork unitOfWork)
    {
        _promotions = promotions;
        _games = games;
        _unitOfWork = unitOfWork;
    }

    public async Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken = default)
    {
        var gameIds = request.GameIds.Distinct().ToList();
        var existing = await _games.GetByIdsAsync(gameIds, cancellationToken);

        var missing = gameIds.Except(existing.Select(g => g.Id)).ToList();
        if (missing.Count > 0)
            throw NotFoundException.For("Jogo", string.Join(", ", missing));

        var promotion = new Promotion(request.Name, request.DiscountPercent, request.StartsAt, request.EndsAt, gameIds);

        await _promotions.AddAsync(promotion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return promotion.ToResponse();
    }

    public async Task<IReadOnlyList<PromotionResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var promotions = await _promotions.GetAllAsync(cancellationToken);
        return promotions.Select(p => p.ToResponse()).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotions.GetByIdAsync(id, cancellationToken)
                        ?? throw NotFoundException.For("Promoção", id);

        promotion.Deactivate();
        _promotions.Update(promotion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
