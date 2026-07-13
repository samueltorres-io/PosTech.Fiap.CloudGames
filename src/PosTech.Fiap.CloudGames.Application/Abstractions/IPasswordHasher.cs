namespace PosTech.Fiap.CloudGames.Application.Abstractions;

/// <summary>Gera e verifica hashes de senha (implementado na infraestrutura com BCrypt).</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);
}
