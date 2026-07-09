using FCG.Application.Abstractions;
using FCG.Application.Auth.Dtos;
using FCG.Application.Common.Exceptions;
using FCG.Domain.Repositories;
using FCG.Domain.ValueObjects;

namespace FCG.Application.Auth;

/// <summary>Autenticação de usuários e emissão de token JWT.</summary>
public sealed class AuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUserRepository users, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email);
        var user = await _users.GetByEmailAsync(email.Value, cancellationToken);

        // Mensagem genérica: não revela se o e-mail existe.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Credenciais inválidas.");

        if (!user.Active)
            throw new UnauthorizedException("Usuário inativo.");

        var token = _tokenGenerator.Generate(user);

        return new AuthResponse(token.Token, token.ExpiresAtUtc, user.Id, user.Email.Value, user.Role);
    }
}
