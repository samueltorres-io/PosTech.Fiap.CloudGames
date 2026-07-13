namespace PosTech.Fiap.CloudGames.Application.Common.Exceptions;

/// <summary>Credenciais ausentes ou inválidas (mapeado para HTTP 401).</summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
