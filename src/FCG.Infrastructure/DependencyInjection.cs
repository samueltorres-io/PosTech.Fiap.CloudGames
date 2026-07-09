using FCG.Application.Abstractions;
using FCG.Domain.Repositories;
using FCG.Infrastructure.Persistence;
using FCG.Infrastructure.Persistence.Dapper;
using FCG.Infrastructure.Persistence.Repositories;
using FCG.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        InfrastructureDefaults.EnableLegacyTimestampBehavior();

        var connectionString = configuration["ConnectionStrings:Postgres"]
                               ?? InfrastructureDefaults.DefaultConnectionString;

        services.AddDbContext<FcgDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FcgDbContext>());
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
