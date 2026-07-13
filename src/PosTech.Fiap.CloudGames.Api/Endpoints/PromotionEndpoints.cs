using PosTech.Fiap.CloudGames.Application.Promotions;
using PosTech.Fiap.CloudGames.Application.Promotions.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Api.Endpoints;

public static class PromotionEndpoints
{
    public static IEndpointRouteBuilder MapPromotionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/promotions").WithTags("Promotions")
            .RequireAuthorization(AuthorizationPolicies.Admin);

        group.MapGet("/", async (PromotionService service, CancellationToken cancellationToken) =>
        {
            var promotions = await service.GetAllAsync(cancellationToken);
            return Results.Ok(promotions);
        })
        .WithName("ListPromotions")
        .WithSummary("Lista as promoções (administrador).")
        .Produces<IReadOnlyList<PromotionResponse>>();

        group.MapPost("/", async (
            CreatePromotionRequest request,
            IValidator<CreatePromotionRequest> validator,
            PromotionService service,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var promotion = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/v1/promotions/{promotion.Id}", promotion);
        })
        .WithName("CreatePromotion")
        .WithSummary("Cria uma promoção de desconto para um conjunto de jogos (administrador).")
        .Produces<PromotionResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, PromotionService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        })
        .WithName("DeletePromotion")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
