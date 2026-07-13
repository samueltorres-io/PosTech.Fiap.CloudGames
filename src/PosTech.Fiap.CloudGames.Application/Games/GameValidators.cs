using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Application.Games;

public sealed class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateGameRequestValidator : AbstractValidator<UpdateGameRequest>
{
    public UpdateGameRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
