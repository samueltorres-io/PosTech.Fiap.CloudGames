using FCG.Application.Abstractions;
using FCG.Application.Auth;
using FCG.Application.Auth.Dtos;
using FCG.Application.Common.Exceptions;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using FCG.Domain.Repositories;
using FCG.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace FCG.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwt = new();

    private static User ExistingUser()
        => new("Alice", Email.Create("alice@fcg.com"), "stored-hash", UserRole.User);

    private AuthService CreateSut() => new(_users.Object, _hasher.Object, _jwt.Object);

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var user = ExistingUser();
        _users.Setup(r => r.GetByEmailAsync("alice@fcg.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Str0ng!Pass", "stored-hash")).Returns(true);
        _jwt.Setup(j => j.Generate(user)).Returns(new AccessToken("jwt-token", DateTime.UtcNow.AddHours(1)));
        var sut = CreateSut();

        var response = await sut.LoginAsync(new LoginRequest("alice@fcg.com", "Str0ng!Pass"));

        response.AccessToken.Should().Be("jwt-token");
        response.Email.Should().Be("alice@fcg.com");
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldThrowUnauthorized()
    {
        var user = ExistingUser();
        _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var sut = CreateSut();

        var act = () => sut.LoginAsync(new LoginRequest("alice@fcg.com", "wrong"));

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ShouldThrowUnauthorized()
    {
        _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var sut = CreateSut();

        var act = () => sut.LoginAsync(new LoginRequest("ghost@fcg.com", "whatever"));

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
