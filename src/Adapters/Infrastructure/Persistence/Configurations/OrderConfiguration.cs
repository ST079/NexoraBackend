using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        builder.OwnsOne(o => o.Subtotal, m =>
        {
            m.Property(x => x.Amount).HasColumnName("subtotal_amount").HasColumnType("numeric(18,2)");
            m.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3);
        });
        builder.OwnsOne(o => o.TaxAmount, m =>
            m.Property(x => x.Amount).HasColumnName("tax_amount").HasColumnType("numeric(18,2)"));
        builder.OwnsOne(o => o.ShippingCost, m =>
            m.Property(x => x.Amount).HasColumnName("shipping_cost").HasColumnType("numeric(18,2)"));
        builder.OwnsOne(o => o.DiscountAmount, m =>
            m.Property(x => x.Amount).HasColumnName("discount_amount").HasColumnType("numeric(18,2)"));
        builder.OwnsOne(o => o.Total, m =>
            m.Property(x => x.Amount).HasColumnName("total_amount").HasColumnType("numeric(18,2)"));

        builder.OwnsOne(o => o.ShippingAddress, addr =>
        {
            addr.Property(a => a.Line1).HasColumnName("shipping_line1").HasMaxLength(200);
            addr.Property(a => a.Line2).HasColumnName("shipping_line2").HasMaxLength(200);
            addr.Property(a => a.City).HasColumnName("shipping_city").HasMaxLength(100);
            addr.Property(a => a.State).HasColumnName("shipping_state").HasMaxLength(100);
            addr.Property(a => a.PostalCode).HasColumnName("shipping_postal_code").HasMaxLength(20);
            addr.Property(a => a.Country).HasColumnName("shipping_country").HasMaxLength(2);
        });

        builder.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId);
        builder.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId);
        builder.HasOne(o => o.Payment).WithOne().HasForeignKey<Payment>(p => p.OrderId);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => new { o.UserId, o.Status });
        builder.HasIndex(o => o.CreatedAt);
    }
}