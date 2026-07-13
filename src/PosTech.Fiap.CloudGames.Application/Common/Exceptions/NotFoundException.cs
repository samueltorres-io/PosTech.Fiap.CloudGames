namespace PosTech.Fiap.CloudGames.Application.Common.Exceptions;

/// <summary>Recurso não encontrado (mapeado para HTTP 404).</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, object key)
        => new($"{entity} '{key}' não encontrado(a).");
}
