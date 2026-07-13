using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Application.Users;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(Password.MinLength)
            .WithMessage($"A senha deve ter no mínimo {Password.MinLength} caracteres.")
            .Matches("[A-Za-z]").WithMessage("A senha deve conter ao menos uma letra.")
            .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.")
            .Matches("[^A-Za-z0-9]").WithMessage("A senha deve conter ao menos um caractere especial.");
    }
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}
