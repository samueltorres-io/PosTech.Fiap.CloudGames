using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence;

/// <summary>
/// Usado pelo <c>dotnet ef</c> em tempo de design para criar o contexto sem subir a aplicação.
/// </summary>
public sealed class CloudGamesDbContextFactory : IDesignTimeDbContextFactory<CloudGamesDbContext>
{
    public CloudGamesDbContext CreateDbContext(string[] args)
    {
        InfrastructureDefaults.EnableLegacyTimestampBehavior();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
                               ?? InfrastructureDefaults.DefaultConnectionString;

        var options = new DbContextOptionsBuilder<CloudGamesDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new CloudGamesDbContext(options);
    }
}
