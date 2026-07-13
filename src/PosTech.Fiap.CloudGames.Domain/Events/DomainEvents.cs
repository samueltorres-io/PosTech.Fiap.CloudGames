using PosTech.Fiap.CloudGames.Domain.Common;

namespace PosTech.Fiap.CloudGames.Domain.Events;

/// <summary>Um novo usuário foi cadastrado na plataforma.</summary>
public sealed record UserRegistered(Guid UserId, string Email) : IDomainEvent;

/// <summary>Um novo jogo foi cadastrado no catálogo.</summary>
public sealed record GameCreated(Guid GameId, string Title) : IDomainEvent;

/// <summary>Um jogo foi adquirido e adicionado à biblioteca do usuário.</summary>
public sealed record GameAcquired(Guid UserId, Guid GameId) : IDomainEvent;

/// <summary>Uma promoção foi criada por um administrador.</summary>
public sealed record PromotionCreated(Guid PromotionId, string Name) : IDomainEvent;
