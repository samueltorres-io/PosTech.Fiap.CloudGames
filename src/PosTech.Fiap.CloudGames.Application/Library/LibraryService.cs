using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using PosTech.Fiap.CloudGames.Application.Library.Dtos;
using PosTech.Fiap.CloudGames.Domain.Repositories;

namespace PosTech.Fiap.CloudGames.Application.Library;

/// <summary>Casos de uso da biblioteca do usuário: aquisição e consulta.</summary>
public sealed class LibraryService
{
    private readonly IUserRepository _users;
    private readonly IGameRepository _games;
    private readonly IPromotionRepository _promotions;
    private readonly IGameReadQueries _readQueries;
    private readonly IUnitOfWork _unitOfWork;

    public LibraryService(
        IUserRepository users,
        IGameRepository games,
        IPromotionRepository promotions,
        IGameReadQueries readQueries,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _games = games;
        _promotions = promotions;
        _readQueries = readQueries;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Adiciona um jogo à biblioteca do usuário, aplicando a promoção ativa (se houver).</summary>
    public async Task<AcquisitionResponse> AcquireAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetWithLibraryAsync(userId, cancellationToken)
                   ?? throw NotFoundException.For("Usuário", userId);

        var game = await _games.GetByIdAsync(gameId, cancellationToken);
        if (game is null || !game.Active)
            throw NotFoundException.For("Jogo", gameId);

        if (user.OwnsGame(gameId))
            throw new ConflictException("O usuário já possui este jogo na biblioteca.");

        var now = DateTime.UtcNow;
        var promotion = await _promotions.GetActivePromotionForGameAsync(gameId, now, cancellationToken);
        var pricePaid = promotion is null ? game.Price : game.Price.ApplyDiscount(promotion.DiscountPercent);

        // O usuário já está rastreado (GetWithLibraryAsync); o change-tracking insere o novo item.
        var item = user.AcquireGame(game, pricePaid);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AcquisitionResponse(
            game.Id,
            game.Title,
            item.PricePaid,
            promotion?.DiscountPercent,
            item.AcquiredAt);
    }

    public Task<IReadOnlyList<LibraryItem>> GetLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
        => _readQueries.GetUserLibraryAsync(userId, cancellationToken);
}
