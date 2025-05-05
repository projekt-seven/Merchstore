using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

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

    /// <summary>
    /// Constructor that accepts DbContextOptions, which allows for configuration to be passed in.
    /// This enables different database providers (SQL Server, In-Memory, etc.) to be used with the same context.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// This method is called when the model for a derived context is being created.
    /// It allows for configuration of entities, relationships, and other model-building activities.
    /// </summary>
    /// <param name="modelBuilder">Provides a simple API for configuring the model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from the current assembly
        // This scans for all IEntityTypeConfiguration implementations and applies them
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Configure a value converter for the Money type
        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount, // Convert Money to decimal for storage
            v => Money.FromSEK(v) // Convert decimal back to Money when reading
        );

        // Apply the converter to the TotalPrice property in Order
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasConversion(moneyConverter);

        // Apply the converter to the UnitPrice property in OrderItem
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasConversion(moneyConverter);
    }
}