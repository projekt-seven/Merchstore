using MerchStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MerchStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for the Customer entity.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        // Valfri koppling till User (nullable FK)
        builder.HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Customer>(c => c.UserId)
            .IsRequired(false);
    }
}
