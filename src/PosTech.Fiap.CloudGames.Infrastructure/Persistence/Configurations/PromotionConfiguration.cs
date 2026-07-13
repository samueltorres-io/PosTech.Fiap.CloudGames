using PosTech.Fiap.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Configurations;

public sealed class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("promotions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(120);
        builder.Property(p => p.DiscountPercent).HasColumnType("numeric(5,2)").IsRequired();
        builder.Property(p => p.StartsAt).IsRequired();
        builder.Property(p => p.EndsAt).IsRequired();
        builder.Property(p => p.Active).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();

        builder.HasMany(p => p.Games)
            .WithOne()
            .HasForeignKey(pg => pg.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Games)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
