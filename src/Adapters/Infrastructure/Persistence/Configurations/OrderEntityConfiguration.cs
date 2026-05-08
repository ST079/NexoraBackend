using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        // Table Name
        builder.ToTable("Orders");

        // Primary Key
        builder.HasKey(order => order.OrderId);

        // Properties
        builder.Property(order => order.OrderId).IsRequired();

        builder.Property(order => order.OrderedBy).IsRequired();

        builder.Property(order => order.Status).IsRequired().HasMaxLength(50).HasDefaultValue("PENDING");

        builder.Property(order => order.ShippingAddressStreet).IsRequired().HasMaxLength(200);

        builder.Property(order => order.ShippingAddressCity).IsRequired().HasMaxLength(100);

        builder.Property(order => order.ShippingAddressCountry).IsRequired().HasMaxLength(100);

        builder.Property(order => order.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(order => order.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");

        builder.Property(order => order.OrderNumber).IsRequired().HasMaxLength(100);

        builder.Property(order => order.PaymentId).IsRequired(false);

        // Optional: Unique Order Number
        builder.HasIndex(order => order.OrderNumber).IsUnique();
    }
}
