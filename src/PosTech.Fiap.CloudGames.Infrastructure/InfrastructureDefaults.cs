namespace PosTech.Fiap.CloudGames.Infrastructure;

public static class InfrastructureDefaults
{
    public const string DefaultConnectionString =
        "Host=localhost;Port=5433;Database=cloudgames;Username=cloudgames;Password=cloudgames";

    /// <summary>
    /// Permite gravar DateTime com Kind=Unspecified/Local em colunas timestamptz.
    /// Deve ser chamado antes do primeiro uso do Npgsql.
    /// </summary>
    public static void EnableLegacyTimestampBehavior()
        => AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
}
