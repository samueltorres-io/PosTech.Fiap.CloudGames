using FCG.Application.Auth;
using FCG.Application.Games;
using FCG.Application.Library;
using FCG.Application.Promotions;
using FCG.Application.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Application;

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
