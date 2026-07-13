using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Domain.Enums;

namespace PosTech.Fiap.CloudGames.Api.Identity;

/// <summary>Extrai os dados do usuário autenticado a partir dos claims do JWT.</summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                        ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => Principal?.FindFirstValue(JwtRegisteredClaimNames.Email)
                            ?? Principal?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public bool IsAdministrator => Principal?.IsInRole(nameof(UserRole.Administrator)) ?? false;
}
