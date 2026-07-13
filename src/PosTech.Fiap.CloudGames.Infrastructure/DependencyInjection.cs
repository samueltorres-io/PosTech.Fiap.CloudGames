using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Domain.Repositories;
using PosTech.Fiap.CloudGames.Infrastructure.Persistence;
using PosTech.Fiap.CloudGames.Infrastructure.Persistence.Dapper;
using PosTech.Fiap.CloudGames.Infrastructure.Persistence.Repositories;
using PosTech.Fiap.CloudGames.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PosTech.Fiap.CloudGames.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        InfrastructureDefaults.EnableLegacyTimestampBehavior();

        var connectionString = configuration["ConnectionStrings:Postgres"]
                               ?? InfrastructureDefaults.DefaultConnectionString;

        services.AddDbContext<CloudGamesDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CloudGamesDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        // Dapper (leitura de alta performance).
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddScoped<IGameReadQueries, GameReadQueries>();

        // Segurança.
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
