using PosTech.Fiap.CloudGames.Application.Auth.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Application.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
