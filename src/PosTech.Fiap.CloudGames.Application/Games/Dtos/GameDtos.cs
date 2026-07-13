namespace PosTech.Fiap.CloudGames.Application.Games.Dtos;

/// <summary>Dados para cadastrar um jogo.</summary>
public sealed record CreateGameRequest(
    string Title,
    string? Description,
    string Genre,
    decimal Price,
    DateOnly? ReleaseDate);

/// <summary>Dados para atualizar um jogo.</summary>
public sealed record UpdateGameRequest(
    string Title,
    string? Description,
    string Genre,
    decimal Price,
    DateOnly? ReleaseDate);

/// <summary>Representação de um jogo retornada pela API.</summary>
public sealed record GameResponse(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    decimal Price,
    DateOnly? ReleaseDate,
    bool Active,
    DateTime CreatedAt);

/// <summary>Filtros do catálogo (consulta Dapper).</summary>
public sealed record GameFilter(
    string? Title = null,
    string? Genre = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int Page = 1,
    int PageSize = 20);

/// <summary>Item do catálogo com preço efetivo após promoção ativa (leitura Dapper).</summary>
public sealed record GameCatalogItem(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    decimal Price,
    decimal? DiscountPercent,
    decimal EffectivePrice,
    DateOnly? ReleaseDate);

/// <summary>Item da biblioteca do usuário (leitura Dapper).</summary>
public sealed record LibraryItem(
    Guid GameId,
    string Title,
    string Genre,
    decimal PricePaid,
    DateTime AcquiredAt);
