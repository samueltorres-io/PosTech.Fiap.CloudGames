using System.Data;

namespace FCG.Infrastructure.Persistence.Dapper;

/// <summary>Cria conexões abertas para consultas Dapper.</summary>
public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
