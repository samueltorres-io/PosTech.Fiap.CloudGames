using PosTech.Fiap.CloudGames.Application.Games;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Api.Endpoints;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/games").WithTags("Games");

        // Catálogo público (leitura via Dapper).
        group.MapGet("/", async (
            string? title,
            string? genre,
            decimal? minPrice,
            decimal? maxPrice,
            int? page,
            int? pageSize,
            GameQueryService service,
            CancellationToken cancellationToken) =>
        {
            var filter = new GameFilter(title, genre, minPrice, maxPrice, page ?? 1, pageSize ?? 20);
            var games = await service.SearchAsync(filter, cancellationToken);
            return Results.Ok(games);
        })
        .AllowAnonymous()
        .WithName("SearchGames")
        .WithSummary("Lista o catálogo de jogos com filtros e preço promocional efetivo.")
        .Produces<IReadOnlyList<GameCatalogItem>>();

        group.MapGet("/{id:guid}", async (Guid id, GameQueryService service, CancellationToken cancellationToken) =>
        {
            var game = await service.GetByIdAsync(id, cancellationToken);
            return Results.Ok(game);
        })
        .AllowAnonymous()
        .WithName("GetGameById")
        .Produces<GameResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (
            CreateGameRequest request,
            IValidator<CreateGameRequest> validator,
            GameCommandService service,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var game = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/v1/games/{game.Id}", game);
        })
        .RequireAuthorization(AuthorizationPolicies.Admin)
        .WithName("CreateGame")
        .WithSummary("Cadastra um novo jogo (administrador).")
        .Produces<GameResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateGameRequest request,
            IValidator<UpdateGameRequest> validator,
            GameCommandService service,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var game = await service.UpdateAsync(id, request, cancellationToken);
            return Results.Ok(game);
        })
        .RequireAuthorization(AuthorizationPolicies.Admin)
        .WithName("UpdateGame")
        .Produces<GameResponse>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, GameCommandService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.Admin)
        .WithName("DeleteGame")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
