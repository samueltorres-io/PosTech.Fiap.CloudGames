using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Application.Users;
using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users").WithTags("Users");

        // Perfil do próprio usuário autenticado.
        group.MapGet("/me", async (ICurrentUser currentUser, UserQueryService service, CancellationToken cancellationToken) =>
        {
            var userId = currentUser.UserId ?? throw new UnauthorizedException("Usuário não autenticado.");
            var user = await service.GetByIdAsync(userId, cancellationToken);
            return Results.Ok(user);
        })
        .RequireAuthorization()
        .WithName("GetMyProfile")
        .WithSummary("Retorna o perfil do usuário autenticado.")
        .Produces<UserResponse>();

        // Administração de usuários (somente administradores).
        var admin = group.MapGroup(string.Empty).RequireAuthorization(AuthorizationPolicies.Admin);

        admin.MapGet("/", async (UserQueryService service, CancellationToken cancellationToken) =>
        {
            var users = await service.GetAllAsync(cancellationToken);
            return Results.Ok(users);
        })
        .WithName("ListUsers")
        .WithSummary("Lista todos os usuários (administrador).")
        .Produces<IReadOnlyList<UserResponse>>();

        admin.MapGet("/{id:guid}", async (Guid id, UserQueryService service, CancellationToken cancellationToken) =>
        {
            var user = await service.GetByIdAsync(id, cancellationToken);
            return Results.Ok(user);
        })
        .WithName("GetUserById")
        .Produces<UserResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPut("/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            IValidator<UpdateUserRequest> validator,
            UserCommandService service,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var user = await service.UpdateAsync(id, request, cancellationToken);
            return Results.Ok(user);
        })
        .WithName("UpdateUser")
        .Produces<UserResponse>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPut("/{id:guid}/role", async (
            Guid id,
            ChangeRoleRequest request,
            UserCommandService service,
            CancellationToken cancellationToken) =>
        {
            var user = await service.ChangeRoleAsync(id, request.Role, cancellationToken);
            return Results.Ok(user);
        })
        .WithName("ChangeUserRole")
        .WithSummary("Altera o nível de acesso do usuário (administrador).")
        .Produces<UserResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", async (Guid id, UserCommandService service, CancellationToken cancellationToken) =>
        {
            await service.DeactivateAsync(id, cancellationToken);
            return Results.NoContent();
        })
        .WithName("DeactivateUser")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
