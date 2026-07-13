using System.Data;
using Dapper;
using Npgsql;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Dapper;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    static NpgsqlConnectionFactory()
    {
        // Mapeia colunas snake_case (release_date) para propriedades PascalCase (ReleaseDate).
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        // Permite materializar colunas 'date' em propriedades DateOnly.
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    public NpgsqlConnectionFactory(string connectionString) => _connectionString = connectionString;

    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
