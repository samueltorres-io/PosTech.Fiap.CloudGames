using FCG.Domain.Entities;

namespace FCG.Application.Abstractions;

/// <summary>Token JWT emitido e seu instante de expiração (UTC).</summary>
public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);

/// <summary>Emite tokens JWT para usuários autenticados.</summary>
public interface IJwtTokenGenerator
{
    AccessToken Generate(User user);
}
