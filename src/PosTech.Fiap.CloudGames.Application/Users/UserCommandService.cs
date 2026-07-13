using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;

namespace PosTech.Fiap.CloudGames.Application.Users;

/// <summary>Casos de uso de escrita de usuários (cadastro e administração).</summary>
public sealed class UserCommandService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public UserCommandService(IUserRepository users, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Cadastra um novo usuário comum.</summary>
    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email);

        if (await _users.EmailExistsAsync(email.Value, cancellationToken))
            throw new ConflictException($"O e-mail '{email.Value}' já está cadastrado.");

        // A política de senha é a invariante de domínio; o hash resultante é o que persistimos.
        var password = Password.Create(request.Password);
        var hash = _passwordHasher.Hash(password.Value);

        var user = new User(request.Name, email, hash, UserRole.User);

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Usuário", id);

        user.Rename(request.Name);
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task<UserResponse> ChangeRoleAsync(Guid id, UserRole role, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Usuário", id);

        user.ChangeRole(role);
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken)
                   ?? throw NotFoundException.For("Usuário", id);

        user.Deactivate();
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
