using JwtAuth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtAuth.EntityConfigurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(e => e.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(e => e.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.UnitPrice)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
