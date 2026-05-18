using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;



public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.ProductImageUrl).HasMaxLength(500);
        builder.Property(i => i.Quantity).IsRequired();

        builder.OwnsOne(i => i.UnitPrice, m =>
        {
            m.Property(p => p.Amount).HasColumnName("unit_price_amount")
                .HasColumnType("numeric(18,2)").IsRequired();
            m.Property(p => p.Currency).HasColumnName("unit_price_currency")
                .HasMaxLength(3).IsRequired();
        });

        // Prevent duplicate product entries in the same cart
        builder.HasIndex(i => new { i.CartId, i.ProductId })
            .IsUnique().HasDatabaseName("idx_cart_items_cart_product");
    }
}