

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        //table name
        builder.ToTable("Users");

        // Primary Key
        builder.HasKey(user => user.Id);

        // Properties
        builder.Property(user => user.Id).IsRequired();

        builder.Property(user => user.Name).IsRequired().HasMaxLength(150);

        builder.Property(user => user.Email).IsRequired().HasMaxLength(200);

        // Unique Email
        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.Password).IsRequired().HasMaxLength(500);

        // Address Fields
        builder.Property(user => user.Street).IsRequired().HasMaxLength(200);

        builder.Property(user => user.City).IsRequired().HasMaxLength(100);

        builder.Property(user => user.Country).IsRequired().HasMaxLength(100);

        builder.Property(user => user.PhoneNumber).HasMaxLength(20);

        builder.Property(user => user.ProfileImageUrl).HasMaxLength(500).IsRequired(false);

        builder.Property(user => user.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");

        builder.Property(user => user.IsActive).IsRequired().HasDefaultValue(true);
    }
}
