using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using myshop.Entities.Models;

namespace myshop.DAL.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("ShoppingCarts");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Count)
            .IsRequired();

        builder.HasOne(sc => sc.Product)
            .WithMany()
            .HasForeignKey(sc => sc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.ApplicationUser)
            .WithMany()
            .HasForeignKey(sc => sc.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Matching query filter: automatically exclude cart items whose product was soft-deleted
        builder.HasQueryFilter(sc => !sc.Product.IsDeleted);
    }
}
