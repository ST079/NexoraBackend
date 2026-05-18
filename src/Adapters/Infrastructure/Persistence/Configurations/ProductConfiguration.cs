using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(5000);
        builder.Property(p => p.StockQuantity).HasColumnName("stock_quantity");
        builder.Property(p => p.IsActive).HasColumnName("is_active");
        builder.Property(p => p.IsFeatured).HasColumnName("is_featured");
        builder.Property(p => p.IsDeleted).HasColumnName("is_deleted");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        // Value object: Money (owned entity pattern)
        builder.OwnsOne(p => p.Price, money =>
        {
            money.Property(m => m.Amount).HasColumnName("price_amount")
                .HasColumnType("numeric(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("price_currency")
                .HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(p => p.CompareAtPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("compare_price_amount")
                .HasColumnType("numeric(18,2)");
            money.Property(m => m.Currency).HasColumnName("compare_price_currency")
                .HasMaxLength(3);
        });

        // Value object: Slug
        builder.OwnsOne(p => p.Slug, slug =>
        {
            slug.Property(s => s.Value)
                .HasColumnName("slug")
                .HasMaxLength(300)
                .IsRequired();

            slug.HasIndex(s => s.Value)
                .IsUnique()
                .HasDatabaseName("idx_products_slug");
        });

        // Owned collection: image URLs as JSON
        builder.Property<List<string>>("_imageUrls")
            .HasColumnName("image_urls")
            .HasColumnType("jsonb")
            .HasField("_imageUrls");

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);

        // Indexes
        builder.HasIndex(p => new { p.CategoryId, p.IsActive, p.IsDeleted })
            .HasDatabaseName("idx_products_category_active");
    }
}