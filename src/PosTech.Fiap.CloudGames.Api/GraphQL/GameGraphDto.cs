namespace PosTech.Fiap.CloudGames.Api.GraphQL;

/// <summary>Projeção de jogo exposta via GraphQL (permite filtragem/ordenação dinâmicas).</summary>
public sealed class GameGraphDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateOnly? ReleaseDate { get; init; }
}
