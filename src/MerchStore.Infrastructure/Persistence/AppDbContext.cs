using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using System.Text.Json;

namespace MerchStore.Infrastructure.Persistence;

/// <summary>
/// The database context that provides access to the database through Entity Framework Core.
/// This is the central class in EF Core and serves as the primary point of interaction with the database.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// DbSet represents a collection of entities of a specific type in the database.
    /// Each DbSet typically corresponds to a database table.
    /// </summary>
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Constructor that accepts DbContextOptions, which allows for configuration to be passed in.
    /// This enables different database providers (SQL Server, In-Memory, etc.) to be used with the same context.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// This method is called when the model for a derived context is being created.
    /// It allows for configuration of entities, relationships, and other model-building activities.
    /// </summary>
    /// <param name="modelBuilder">Provides a simple API for configuring the model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Money converter
        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount, // Convert Money to decimal for storage
            v => Money.FromSEK(v) // Convert decimal back to Money when reading
        );

        // Configure Order entity
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey("CustomerId")
            .IsRequired(); // Ensure Customer is required

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasConversion(moneyConverter);

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasConversion(moneyConverter);

        // Tags converter & comparer for Product.Tags
        var tagsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());

        var tagsComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        modelBuilder.Entity<Product>()
            .Property(p => p.Tags)
            .HasConversion(tagsConverter, tagsComparer);
    }
}
