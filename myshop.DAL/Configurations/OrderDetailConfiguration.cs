using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using myshop.Entities.Models;

namespace myshop.DAL.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");

        builder.HasKey(od => od.Id);

        builder.Property(od => od.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(od => od.Count)
            .IsRequired();

        builder.HasOne(od => od.OrderHeader)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderHeaderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
