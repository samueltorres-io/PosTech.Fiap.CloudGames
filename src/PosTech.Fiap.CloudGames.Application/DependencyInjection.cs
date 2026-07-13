using PosTech.Fiap.CloudGames.Application.Auth;
using PosTech.Fiap.CloudGames.Application.Games;
using PosTech.Fiap.CloudGames.Application.Library;
using PosTech.Fiap.CloudGames.Application.Promotions;
using PosTech.Fiap.CloudGames.Application.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PosTech.Fiap.CloudGames.Application;

public static class DependencyInjection
{
    /// <summary>Registra os serviços de caso de uso e os validadores da camada de aplicação.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddScoped<AuthService>();
        services.AddScoped<UserCommandService>();
        services.AddScoped<UserQueryService>();
        services.AddScoped<GameCommandService>();
        services.AddScoped<GameQueryService>();
        services.AddScoped<LibraryService>();
        services.AddScoped<PromotionService>();

        return services;
    }
}
