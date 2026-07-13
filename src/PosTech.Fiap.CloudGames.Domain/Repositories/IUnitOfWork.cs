namespace PosTech.Fiap.CloudGames.Domain.Repositories;

/// <summary>Confirma, de forma atômica, as alterações pendentes no repositório.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
