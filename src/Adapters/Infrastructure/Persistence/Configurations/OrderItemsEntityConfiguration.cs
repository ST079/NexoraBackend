using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        // Table Name
        builder.ToTable("OrderItems");

        // Primary Key
        builder.HasKey(x => x.OrderItemId);

        // Properties
        builder.Property(x => x.OrderItemId)
            .IsRequired();

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.PriceAtTime)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Optional: Prevent duplicate same product in same order
        builder.HasIndex(x => new { x.OrderId, x.ProductId })
            .IsUnique();

        // Foreign Key: Order → OrderItems
        builder.HasOne<OrderEntity>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign Key: Product → OrderItems
        builder.HasOne<ProductEntity>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}