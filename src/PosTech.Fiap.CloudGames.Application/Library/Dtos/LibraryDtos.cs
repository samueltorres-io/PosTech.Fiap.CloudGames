namespace PosTech.Fiap.CloudGames.Application.Library.Dtos;

/// <summary>Resultado da aquisição de um jogo.</summary>
public sealed record AcquisitionResponse(
    Guid GameId,
    string Title,
    decimal PricePaid,
    decimal? DiscountApplied,
    DateTime AcquiredAt);
