using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        // Table Name
        builder.ToTable("Products");

        //primary key
        builder.HasKey(product => product.ProductId);

        builder.Property(product => product.ProductId).ValueGeneratedOnAdd();

        builder.Property(product => product.Name).IsRequired().HasMaxLength(200);

        // Description
        builder.Property(product => product.Description).HasMaxLength(1000);

        // Brand
        builder.Property(product => product.Brand).HasMaxLength(150);

        // Category
        builder.Property(product => product.Category).HasMaxLength(150);

        // Price
        builder.Property(product => product.Price).IsRequired().HasColumnType("decimal(18,2)");

        // Stock
        builder.Property(product => product.Stock).IsRequired();

        // CreatedAt
        builder.Property(product => product.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");

        // CreatedBy
        builder.Property(product => product.CreatedBy).IsRequired();

        // ImageUrls
        // PostgreSQL supports text[] array using Npgsql
        builder.Property(product => product.ImageUrls).HasColumnType("text[]");
    }
}
