using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;


public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("coupons");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.DiscountValue).HasColumnType("numeric(10,4)").IsRequired();
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");

        builder.OwnsOne(c => c.MaximumDiscountAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("max_discount_amount").HasColumnType("numeric(18,2)");
            m.Property(p => p.Currency).HasColumnName("max_discount_currency").HasMaxLength(3);
        });

        builder.OwnsOne(c => c.MinimumOrderAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("min_order_amount").HasColumnType("numeric(18,2)");
            m.Property(p => p.Currency).HasColumnName("min_order_currency").HasMaxLength(3);
        });

        builder.HasIndex(c => c.Code).IsUnique().HasDatabaseName("idx_coupons_code");
        builder.HasIndex(c => new { c.IsActive, c.ExpiresAt }).HasDatabaseName("idx_coupons_active_expiry");
    }
}