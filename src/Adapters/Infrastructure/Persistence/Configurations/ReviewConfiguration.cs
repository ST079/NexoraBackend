using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;


public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Body).HasMaxLength(2000).IsRequired();
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.IsVerifiedPurchase).HasDefaultValue(false);
        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");

        builder.OwnsOne(r => r.Rating, rating =>
        {
            rating.Property(ra => ra.Value).HasColumnName("rating").IsRequired();
        });

        builder.HasOne(r => r.Product).WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User).WithMany()
            .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.HelpfulVotes).WithOne()
            .HasForeignKey(v => v.ReviewId).OnDelete(DeleteBehavior.Cascade);

        // One review per user per product (enforced at DB level too)
        builder.HasIndex(r => new { r.UserId, r.ProductId })
            .IsUnique().HasDatabaseName("idx_reviews_user_product");

        builder.HasIndex(r => new { r.ProductId, r.Status })
            .HasDatabaseName("idx_reviews_product_status");
    }
}