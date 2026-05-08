
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.RoleId);
        builder.Property(role => role.RoleId).ValueGeneratedOnAdd();
        builder.Property(role => role.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(role => role.RoleId).IsUnique();

    }
}