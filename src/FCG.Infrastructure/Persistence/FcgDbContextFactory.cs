using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FCG.Infrastructure.Persistence;

/// <summary>
/// Usado pelo <c>dotnet ef</c> em tempo de design para criar o contexto sem subir a aplicação.
/// </summary>
public sealed class FcgDbContextFactory : IDesignTimeDbContextFactory<FcgDbContext>
{
    public FcgDbContext CreateDbContext(string[] args)
    {
        InfrastructureDefaults.EnableLegacyTimestampBehavior();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
                               ?? InfrastructureDefaults.DefaultConnectionString;

        var options = new DbContextOptionsBuilder<FcgDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new FcgDbContext(options);
    }
}
