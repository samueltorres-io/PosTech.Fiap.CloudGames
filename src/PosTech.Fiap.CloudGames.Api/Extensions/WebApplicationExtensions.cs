using PosTech.Fiap.CloudGames.Api.Endpoints;
using PosTech.Fiap.CloudGames.Api.Middleware;
using PosTech.Fiap.CloudGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace PosTech.Fiap.CloudGames.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        // Primeiro middleware: captura qualquer exceção e devolve ProblemDetails.
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseSerilogRequestLogging();

        // Requisito (Desafio Fase 1 · RT-07): documentação Swagger dos endpoints.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    // Requisito (Desafio Fase 1 · RT-05): API .NET 8 no padrão Minimal API,
    // com endpoints agrupados por feature.
    public static WebApplication MapFcgEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).WithTags("Health").AllowAnonymous();

        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapGameEndpoints();
        app.MapLibraryEndpoints();
        app.MapPromotionEndpoints();

        app.MapGraphQL();

        return app;
    }

    /// <summary>Aplica as migrations pendentes e popula os dados iniciais.</summary>
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CloudGamesDbContext>();
        await context.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}
