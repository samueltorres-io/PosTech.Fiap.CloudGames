using FCG.Application.Users.Dtos;
using FCG.Domain.Entities;

namespace FCG.Application.Users;

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
