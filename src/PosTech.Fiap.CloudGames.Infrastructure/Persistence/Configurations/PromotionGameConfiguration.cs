using PosTech.Fiap.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Configurations;

public sealed class PromotionGameConfiguration : IEntityTypeConfiguration<PromotionGame>
{
    public void Configure(EntityTypeBuilder<PromotionGame> builder)
    {
        builder.ToTable("promotion_games");

        builder.HasKey(pg => pg.Id);
        builder.Property(pg => pg.Id).ValueGeneratedNever();

        builder.Property(pg => pg.PromotionId).IsRequired();
        builder.Property(pg => pg.GameId).IsRequired();

        builder.HasIndex(pg => new { pg.PromotionId, pg.GameId }).IsUnique();

        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(pg => pg.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
