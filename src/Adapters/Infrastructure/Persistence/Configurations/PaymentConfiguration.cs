using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
        builder.Property(p => p.StripePaymentIntentId).HasColumnName("stripe_payment_intent_id").HasMaxLength(200);
        builder.Property(p => p.StripeChargeId).HasColumnName("stripe_charge_id").HasMaxLength(200);
        builder.Property(p => p.FailureReason).HasColumnName("failure_reason").HasMaxLength(500);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.OwnsOne(p => p.Amount, m =>
        {
            m.Property(a => a.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
            m.Property(a => a.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        });

        builder.HasIndex(p => p.StripePaymentIntentId)
            .IsUnique().HasFilter("stripe_payment_intent_id IS NOT NULL")
            .HasDatabaseName("idx_payments_stripe_intent");

        builder.HasIndex(p => p.OrderId).IsUnique()
            .HasDatabaseName("idx_payments_order"); // one payment per order
    }
}
