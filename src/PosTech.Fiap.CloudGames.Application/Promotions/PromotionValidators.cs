using PosTech.Fiap.CloudGames.Application.Promotions.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Application.Promotions;

public sealed class CreatePromotionRequestValidator : AbstractValidator<CreatePromotionRequest>
{
    public CreatePromotionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0.01m, 100m);
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt)
            .WithMessage("A data de término deve ser posterior à data de início.");
        RuleFor(x => x.GameIds).NotEmpty().WithMessage("Informe ao menos um jogo para a promoção.");
    }
}
