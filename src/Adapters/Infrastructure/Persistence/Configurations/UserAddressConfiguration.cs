using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;


public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("user_addresses");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Label).HasMaxLength(50).IsRequired();
        builder.Property(a => a.IsDefault).HasDefaultValue(false);

        // Embed the Address value object as owned columns
        builder.OwnsOne(a => a.Address, addr =>
        {
            addr.Property(x => x.Line1).HasColumnName("line1").HasMaxLength(200).IsRequired();
            addr.Property(x => x.Line2).HasColumnName("line2").HasMaxLength(200);
            addr.Property(x => x.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            addr.Property(x => x.State).HasColumnName("state").HasMaxLength(100);
            addr.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(20).IsRequired();
            addr.Property(x => x.Country).HasColumnName("country").HasMaxLength(2).IsRequired();
        });

        builder.HasIndex(a => new { a.UserId, a.IsDefault })
            .HasDatabaseName("idx_user_addresses_user_default");
    }
}