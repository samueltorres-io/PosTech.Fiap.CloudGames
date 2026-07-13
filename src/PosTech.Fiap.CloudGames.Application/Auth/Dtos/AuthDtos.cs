using PosTech.Fiap.CloudGames.Domain.Enums;

namespace PosTech.Fiap.CloudGames.Application.Auth.Dtos;

/// <summary>Credenciais de login.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Token emitido e dados essenciais do usuário autenticado.</summary>
public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    UserRole Role);
