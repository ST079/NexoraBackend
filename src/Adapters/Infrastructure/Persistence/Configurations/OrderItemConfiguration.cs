using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items"); builder.HasKey(i => i.Id); builder.Property(i => i.ProductName).HasMaxLength(200).IsRequired(); builder.Property(i => i.Quantity).IsRequired();

        builder.OwnsOne(i => i.UnitPrice, m =>
        {
            m.Property(p => p.Amount).HasColumnName("unit_price_amount")
                .HasColumnType("numeric(18,2)").IsRequired();
            m.Property(p => p.Currency).HasColumnName("unit_price_currency")
                .HasMaxLength(3).IsRequired();
        });

        // LineTotal is computed — not stored; EF ignores it automatically
        builder.Ignore(i => i.LineTotal);

        builder.HasIndex(i => i.OrderId).HasDatabaseName("idx_order_items_order");
    }
}