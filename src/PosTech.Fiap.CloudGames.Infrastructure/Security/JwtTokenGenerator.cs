using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace PosTech.Fiap.CloudGames.Infrastructure.Security;

/// <summary>
/// Requisito (Desafio Fase 1 · RF-03): "implementar autenticação via token JWT".
/// Emite o token com os claims de identidade e o papel (role) do usuário.
/// </summary>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> options) => _settings = options.Value;

    public AccessToken Generate(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new AccessToken(jwt, expiresAt);
    }
}
