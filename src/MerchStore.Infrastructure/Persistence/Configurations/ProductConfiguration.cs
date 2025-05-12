using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MerchStore.Domain.Entities;
using System.Text.Json;

namespace MerchStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration class for the Product entity.
/// This defines how a Product is mapped to the database.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the entity mapping using EF Core's Fluent API.
    /// </summary>
    /// <param name="builder">Entity type builder used to configure the entity</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Define the table name explicitly
        builder.ToTable("Products");

        // Configure the primary key
        builder.HasKey(p => p.Id);

        // Configure Name property
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Configure Description property
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        // Configure StockQuantity property
        builder.Property(p => p.StockQuantity)
            .IsRequired();

        // Configure ImageUrl property - it's nullable
        builder.Property(p => p.ImageUrl)
            .IsRequired(false);

        // Configure the owned entity Money as a complex type
        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(m => m.Amount)
                .HasColumnName("Price")
                .IsRequired();

            priceBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Add an index on the Name for faster lookups
        builder.HasIndex(p => p.Name);

        // Configure Tags property with value converter and value comparer
        var tagsProperty = builder.Property(p => p.Tags);

        tagsProperty
            .HasConversion(
                new ValueConverter<List<string>, string>(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                )
            )
            .HasColumnType("TEXT");

        tagsProperty.Metadata.SetValueComparer(
            new ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? new List<string>() : c.ToList()
            )
        );
    }
}
