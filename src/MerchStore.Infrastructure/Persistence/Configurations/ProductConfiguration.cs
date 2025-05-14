using MerchStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace MerchStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for the Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Set the primary key for the Product entity
        builder.HasKey(p => p.Id);

        // Configure the Name property (required, max length 200 characters)
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Configure the Description property (optional, max length 1000 characters)
        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        // Configure the owned value object 'Price'
        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            // Map the 'Amount' property of Money to a column named 'Price'
            priceBuilder.Property(m => m.Amount)
                .HasColumnName("Price")
                .IsRequired();
        });

        // ValueConverter to serialize/deserialize Tags as JSON in the database
        var tagsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), // Convert List<string> to JSON string
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null!) ?? new() // Convert JSON back to List<string>
        );

        // ValueComparer to compare List<string> values by content rather than reference
        var tagsComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2), // Equality comparison based on sequence
            c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Generate hash code
            c => c == null ? new List<string>() : c.ToList() // Deep copy for snapshotting
        );

        // Apply the converter and comparer to the Tags property
        builder.Property(p => p.Tags)
            .HasConversion(tagsConverter)
            .Metadata.SetValueComparer(tagsComparer);

        // Create an index on the Name column for performance
        builder.HasIndex(p => p.Name);
    }
} 
