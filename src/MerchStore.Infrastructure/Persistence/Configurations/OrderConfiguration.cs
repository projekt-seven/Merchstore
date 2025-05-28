using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Define the table name explicitly
        builder.ToTable("Orders");

        // Configure the primary key
        builder.HasKey(o => o.Id);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        // Configure the OrderDate property
        builder.Property(o => o.OrderDate)
            .IsRequired(); // NOT NULL constraint

        // Configure the TotalPrice property
        builder.Property(o => o.TotalPrice)
            .IsRequired(); // NOT NULL constraint

        // Configure the Status property
        builder.Property(o => o.Status)
            .IsRequired(); // NOT NULL constraint

        // Add an index on the OrderDate for faster lookups
        builder.HasIndex(o => o.OrderDate);
    }
}