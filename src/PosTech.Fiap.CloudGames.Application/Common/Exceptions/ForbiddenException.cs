namespace PosTech.Fiap.CloudGames.Application.Common.Exceptions;

/// <summary>Ação não permitida para o usuário atual (mapeado para HTTP 403).</summary>
public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
