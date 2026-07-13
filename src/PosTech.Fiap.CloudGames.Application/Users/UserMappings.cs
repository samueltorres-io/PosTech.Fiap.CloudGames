using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;

namespace PosTech.Fiap.CloudGames.Application.Users;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user) => new(
        user.Id,
        user.Name,
        user.Email.Value,
        user.Role,
        user.Active,
        user.CreatedAt);
}
