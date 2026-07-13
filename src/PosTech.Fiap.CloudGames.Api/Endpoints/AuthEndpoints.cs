using PosTech.Fiap.CloudGames.Application.Auth;
using PosTech.Fiap.CloudGames.Application.Auth.Dtos;
using PosTech.Fiap.CloudGames.Application.Users;
using PosTech.Fiap.CloudGames.Application.Users.Dtos;
using FluentValidation;

namespace PosTech.Fiap.CloudGames.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterUserRequest request,
            IValidator<RegisterUserRequest> validator,
            UserCommandService service,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.RegisterAsync(request, cancellationToken);
            return Results.Created($"/api/v1/users/{response.Id}", response);
        })
        .AllowAnonymous()
        .WithName("RegisterUser")
        .WithSummary("Cadastra um novo usuário.")
        .Produces<UserResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", async (
            LoginRequest request,
            IValidator<LoginRequest> validator,
            AuthService service,
            CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var response = await service.LoginAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .AllowAnonymous()
        .WithName("Login")
        .WithSummary("Autentica o usuário e retorna um token JWT.")
        .Produces<AuthResponse>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
