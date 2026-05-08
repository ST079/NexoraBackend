using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Table Name
        builder.ToTable("Payments");

        // Primary Key
        builder.HasKey(x => x.PaymentId);

        builder.Property(x => x.PaymentId)
            .IsRequired();

        builder.Property(x => x.TransactionId)
            .HasMaxLength(200)
            .IsRequired(false);

        // Optional: Prevent duplicate transaction IDs
        builder.HasIndex(x => x.TransactionId)
            .IsUnique();

        builder.Property(x => x.Method)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
    }
}