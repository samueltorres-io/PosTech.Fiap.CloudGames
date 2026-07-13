using PosTech.Fiap.CloudGames.Domain.Entities;
using PosTech.Fiap.CloudGames.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PosTech.Fiap.CloudGames.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();
        builder.Ignore(u => u.DomainEvents);

        builder.Property(u => u.Name).IsRequired().HasMaxLength(120);

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .IsRequired()
            .HasMaxLength(320);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().IsRequired().HasMaxLength(20);
        builder.Property(u => u.Active).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.HasMany(u => u.Library)
            .WithOne()
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.Library)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
