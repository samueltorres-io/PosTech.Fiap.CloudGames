using PosTech.Fiap.CloudGames.Domain.Common;
using PosTech.Fiap.CloudGames.Domain.Events;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;

namespace PosTech.Fiap.CloudGames.Domain.Entities;

/// <summary>Raiz de agregado: um jogo do catálogo da PosTech.Fiap.CloudGames.</summary>
public sealed class Game : Entity
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public string Genre { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public DateOnly? ReleaseDate { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Construtor para o EF Core.
    private Game()
    {
    }

    public Game(string title, string? description, string genre, Money price, DateOnly? releaseDate = null)
    {
        Rename(title);
        ChangeGenre(genre);
        Description = description?.Trim() ?? string.Empty;
        Price = price ?? throw new DomainException("O preço é obrigatório.");
        ReleaseDate = releaseDate;
        Active = true;
        CreatedAt = DateTime.UtcNow;

        Raise(new GameCreated(Id, Title));
    }

    public void Update(string title, string? description, string genre, Money price, DateOnly? releaseDate)
    {
        Rename(title);
        ChangeGenre(genre);
        Description = description?.Trim() ?? string.Empty;
        Price = price ?? throw new DomainException("O preço é obrigatório.");
        ReleaseDate = releaseDate;
    }

    public void Activate() => Active = true;

    public void Deactivate() => Active = false;

    private void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título do jogo é obrigatório.");

        Title = title.Trim();
    }

    private void ChangeGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            throw new DomainException("O gênero do jogo é obrigatório.");

        Genre = genre.Trim();
    }
}
