using System.Data;
using Dapper;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Dapper;

/// <summary>
/// Converte entre a coluna PostgreSQL <c>date</c> (lida como DateTime pelo Npgsql) e <see cref="DateOnly"/>.
/// Necessário porque o Dapper não converte DateTime → DateOnly automaticamente.
/// </summary>
public sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value) => value switch
    {
        DateTime dateTime => DateOnly.FromDateTime(dateTime),
        DateOnly dateOnly => dateOnly,
        _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
    };
}
