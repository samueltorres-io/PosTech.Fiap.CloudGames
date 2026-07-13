using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Auth;
using PosTech.Fiap.CloudGames.Application.Auth.Dtos;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace PosTech.Fiap.CloudGames.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwt = new();

    private static User ExistingUser()
        => new("Alice", Email.Create("alice@cloudgames.com"), "stored-hash", UserRole.User);

    private AuthService CreateSut() => new(_users.Object, _hasher.Object, _jwt.Object);

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var user = ExistingUser();
        _users.Setup(r => r.GetByEmailAsync("alice@cloudgames.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Str0ng!Pass", "stored-hash")).Returns(true);
        _jwt.Setup(j => j.Generate(user)).Returns(new AccessToken("jwt-token", DateTime.UtcNow.AddHours(1)));
        var sut = CreateSut();

        var response = await sut.LoginAsync(new LoginRequest("alice@cloudgames.com", "Str0ng!Pass"));

        response.AccessToken.Should().Be("jwt-token");
        response.Email.Should().Be("alice@cloudgames.com");
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldThrowUnauthorized()
    {
        var user = ExistingUser();
        _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var sut = CreateSut();

        var act = () => sut.LoginAsync(new LoginRequest("alice@cloudgames.com", "wrong"));

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ShouldThrowUnauthorized()
    {
        _users.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var sut = CreateSut();

        var act = () => sut.LoginAsync(new LoginRequest("ghost@cloudgames.com", "whatever"));

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
