using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Configurations;

public sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).ValueGeneratedNever();
        builder.Ignore(g => g.DomainEvents);

        builder.Property(g => g.Title).IsRequired().HasMaxLength(200);
        builder.HasIndex(g => g.Title).IsUnique();

        builder.Property(g => g.Description).HasMaxLength(2000);
        builder.Property(g => g.Genre).IsRequired().HasMaxLength(80);

        // Money -> coluna decimal (moeda única BRL neste MVP).
        builder.Property(g => g.Price)
            .HasConversion(price => price.Amount, value => Money.Create(value, Money.DefaultCurrency))
            .HasColumnName("price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(g => g.ReleaseDate);
        builder.Property(g => g.Active).IsRequired();
        builder.Property(g => g.CreatedAt).IsRequired();
    }
}
