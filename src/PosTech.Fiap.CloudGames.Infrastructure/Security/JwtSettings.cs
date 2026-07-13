namespace PosTech.Fiap.CloudGames.Infrastructure.Security;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "postech-fiap-cloudgames";
    public string Audience { get; init; } = "fcg-clients";
    public string SecretKey { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 120;
}
