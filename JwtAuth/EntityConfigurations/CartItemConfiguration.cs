using JwtAuth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtAuth.EntityConfigurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(e => e.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Product)
                   .WithMany(p => p.CartItems)
                   .HasForeignKey(e => e.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
