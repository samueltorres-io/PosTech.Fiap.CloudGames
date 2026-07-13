using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentAssertions;

namespace PosTech.Fiap.CloudGames.Domain.Tests.ValueObjects;

public class PasswordTests
{
    [Theory]
    [InlineData("Abcd123!")]
    [InlineData("S3nh@Segura")]
    [InlineData("P@ssw0rd2024")]
    public void Create_WithStrongPassword_ShouldSucceed(string value)
    {
        var password = Password.Create(value);

        password.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("", "obrigatória")]
    [InlineData("Ab1!", "8 caracteres")]           // curta demais
    [InlineData("12345678!", "letra")]             // sem letra
    [InlineData("Password!", "número")]            // sem número
    [InlineData("Password1", "caractere especial")] // sem especial
    public void Create_WithWeakPassword_ShouldThrowWithReason(string value, string expectedReason)
    {
        var act = () => Password.Create(value);

        act.Should().Throw<DomainException>().WithMessage($"*{expectedReason}*");
    }

    [Fact]
    public void ToString_ShouldNotLeakTheRawValue()
    {
        Password.Create("Abcd123!").ToString().Should().NotContain("Abcd123!");
    }
}
