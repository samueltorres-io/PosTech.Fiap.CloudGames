using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Games.Dtos;
using PosTech.Fiap.CloudGames.Application.Library;
using PosTech.Fiap.CloudGames.Application.Library.Dtos;

namespace PosTech.Fiap.CloudGames.Api.Endpoints;

public static class LibraryEndpoints
{
    public static IEndpointRouteBuilder MapLibraryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/library").WithTags("Library").RequireAuthorization();

        group.MapGet("/", async (ICurrentUser currentUser, LibraryService service, CancellationToken cancellationToken) =>
        {
            var userId = currentUser.UserId ?? throw new UnauthorizedException("Usuário não autenticado.");
            var library = await service.GetLibraryAsync(userId, cancellationToken);
            return Results.Ok(library);
        })
        .WithName("GetMyLibrary")
        .WithSummary("Lista os jogos da biblioteca do usuário autenticado.")
        .Produces<IReadOnlyList<LibraryItem>>();

        group.MapPost("/{gameId:guid}", async (
            Guid gameId,
            ICurrentUser currentUser,
            LibraryService service,
            CancellationToken cancellationToken) =>
        {
            var userId = currentUser.UserId ?? throw new UnauthorizedException("Usuário não autenticado.");
            var result = await service.AcquireAsync(userId, gameId, cancellationToken);
            return Results.Created($"/api/v1/library/{result.GameId}", result);
        })
        .WithName("AcquireGame")
        .WithSummary("Adquire um jogo e o adiciona à biblioteca (aplica promoção ativa, se houver).")
        .Produces<AcquisitionResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
