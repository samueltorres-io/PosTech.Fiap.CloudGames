namespace PosTech.Fiap.CloudGames.Application.Common.Exceptions;

/// <summary>Conflito com o estado atual do recurso (mapeado para HTTP 409).</summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
