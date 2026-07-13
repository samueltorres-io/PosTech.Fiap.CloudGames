using PosTech.Fiap.CloudGames.Domain.Enums;

namespace PosTech.Fiap.CloudGames.Application.Users.Dtos;

/// <summary>Dados de cadastro de um novo usuário.</summary>
public sealed record RegisterUserRequest(string Name, string Email, string Password);

/// <summary>Dados para atualizar o nome de um usuário.</summary>
public sealed record UpdateUserRequest(string Name);

/// <summary>Alteração do nível de acesso de um usuário (admin).</summary>
public sealed record ChangeRoleRequest(UserRole Role);

/// <summary>Representação de um usuário retornada pela API.</summary>
public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    bool Active,
    DateTime CreatedAt);
