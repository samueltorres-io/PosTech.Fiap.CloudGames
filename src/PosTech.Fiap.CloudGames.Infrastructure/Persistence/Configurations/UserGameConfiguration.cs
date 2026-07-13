using PosTech.Fiap.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Configurations;

public sealed class UserGameConfiguration : IEntityTypeConfiguration<UserGame>
{
    public void Configure(EntityTypeBuilder<UserGame> builder)
    {
        builder.ToTable("user_games");

        builder.HasKey(ug => ug.Id);
        builder.Property(ug => ug.Id).ValueGeneratedNever();

        builder.Property(ug => ug.UserId).IsRequired();
        builder.Property(ug => ug.GameId).IsRequired();
        builder.Property(ug => ug.PricePaid).HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(ug => ug.AcquiredAt).IsRequired();

        // Um usuário não pode possuir o mesmo jogo duas vezes.
        builder.HasIndex(ug => new { ug.UserId, ug.GameId }).IsUnique();

        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(ug => ug.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
