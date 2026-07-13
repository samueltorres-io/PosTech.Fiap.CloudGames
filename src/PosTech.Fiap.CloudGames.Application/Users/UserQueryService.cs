using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using PosTech.Fiap.CloudGames.Domain.Repositories;

namespace PosTech.Fiap.CloudGames.Application.Users;

/// <summary>Casos de uso de leitura de usuários.</summary>
public sealed class UserQueryService
{
    private readonly IUserRepository _users;

    public UserQueryService(IUserRepository users) => _users = users;

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        return users.Select(u => u.ToResponse()).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Usuário", id);

        return user.ToResponse();
    }
}
