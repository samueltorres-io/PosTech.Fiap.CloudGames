using FCG.Application.Abstractions;
using FCG.Application.Common.Exceptions;
using FCG.Application.Users;
using FCG.Application.Users.Dtos;
using FCG.Domain.Entities;
using FCG.Domain.Exceptions;
using FCG.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace FCG.Application.Tests.Users;

public class UserCommandServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private UserCommandService CreateSut() => new(_users.Object, _hasher.Object, _uow.Object);

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ShouldHashPasswordAndPersistUser()
    {
        _users.Setup(r => r.EmailExistsAsync("alice@fcg.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-secret");
        var sut = CreateSut();

        var response = await sut.RegisterAsync(new RegisterUserRequest("Alice", "alice@fcg.com", "Str0ng!Pass"));

        response.Email.Should().Be("alice@fcg.com");
        _hasher.Verify(h => h.Hash("Str0ng!Pass"), Times.Once);
        _users.Verify(r => r.AddAsync(It.Is<User>(u => u.PasswordHash == "hashed-secret"), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowConflict()
    {
        _users.Setup(r => r.EmailExistsAsync("alice@fcg.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var sut = CreateSut();

        var act = () => sut.RegisterAsync(new RegisterUserRequest("Alice", "alice@fcg.com", "Str0ng!Pass"));

        await act.Should().ThrowAsync<ConflictException>();
        _users.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithWeakPassword_ShouldThrowDomainException()
    {
        _users.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var sut = CreateSut();

        var act = () => sut.RegisterAsync(new RegisterUserRequest("Alice", "alice@fcg.com", "weak"));

        await act.Should().ThrowAsync<DomainException>();
    }
}
