using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentAssertions;

namespace PosTech.Fiap.CloudGames.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@cloudgames.com")]
    [InlineData("john.doe@example.co")]
    [InlineData("a_b+c@sub.domain.org")]
    public void Create_WithValidEmail_ShouldSucceed(string value)
    {
        var email = Email.Create(value);

        email.Value.Should().Be(value.ToLowerInvariant());
    }

    [Fact]
    public void Create_ShouldNormalizeToLowercaseAndTrim()
    {
        var email = Email.Create("  User@CloudGames.COM  ");

        email.Value.Should().Be("user@cloudgames.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => Email.Create(value);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("no-at-sign.com")]
    [InlineData("missing@domain")]
    [InlineData("spaces in@email.com")]
    [InlineData("@no-local.com")]
    public void Create_WithInvalidFormat_ShouldThrow(string value)
    {
        var act = () => Email.Create(value);

        act.Should().Throw<DomainException>().WithMessage("*formato inválido*");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        Email.Create("user@cloudgames.com").Should().Be(Email.Create("USER@cloudgames.com"));
    }
}
