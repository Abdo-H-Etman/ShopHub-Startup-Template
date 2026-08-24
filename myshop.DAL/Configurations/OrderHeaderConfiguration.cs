using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using myshop.Entities.Models;

namespace myshop.DAL.Configurations;

public class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderHeader>
{
    public void Configure(EntityTypeBuilder<OrderHeader> builder)
    {
        builder.ToTable("OrderHeaders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Address)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(o => o.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.PostalCode)
            .HasMaxLength(20);

        builder.Property(o => o.PhoneNumber)
            .HasMaxLength(25);

        builder.Property(o => o.OrderStatus)
            .HasMaxLength(50);

        builder.Property(o => o.PaymentStatus)
            .HasMaxLength(50);

        builder.Property(o => o.Carrier)
            .HasMaxLength(50);

        builder.Property(o => o.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(o => o.SessionId)
            .HasMaxLength(200);

        builder.Property(o => o.PaymentIntentId)
            .HasMaxLength(200);

        builder.Property(o => o.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(o => o.ApplicationUser)
            .WithMany()
            .HasForeignKey(o => o.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderDetails)
            .WithOne(od => od.OrderHeader)
            .HasForeignKey(od => od.OrderHeaderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
