namespace PosTech.Fiap.CloudGames.Application.Abstractions;

/// <summary>Dados do usuário autenticado na requisição atual (extraídos do JWT).</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }

    bool IsAdministrator { get; }
}
