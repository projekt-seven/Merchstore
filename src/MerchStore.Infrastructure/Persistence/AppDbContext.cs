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
    /// DbSet representing the Products table.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// DbSet representing the Orders table.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// DbSet representing the Customers table.
    /// </summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Constructor that accepts DbContextOptions to configure the context.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// This method is called when the model for a derived context is being created.
    /// It allows for configuration of entities, relationships, and conversions.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration implementations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ValueConverter for the Money value object
        // Converts Money to decimal for database storage and back to Money when reading
        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount,
            v => Money.FromSEK(v)
        );

        // Configure relationship: Order requires a Customer
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey("CustomerId")
            .IsRequired();

        // Apply the Money converter to the TotalPrice property on Order
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasConversion(moneyConverter);

        // Apply the Money converter to the UnitPrice property on OrderItem
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasConversion(moneyConverter);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            entity.Property(u => u.CreatedAt).IsRequired();
        });
        
        // ValueConverter for serializing/deserializing Product.Tags as JSON
        var tagsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null!) ?? new()
        );

        // ValueComparer to compare List<string> values for Product.Tags
        var tagsComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2), // Equality check
            c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Hash code
            c => c == null ? new List<string>() : c.ToList() // Snapshot copy
        );

        // Apply both converter and comparer to Product.Tags
        modelBuilder.Entity<Product>()
            .Property(p => p.Tags)
            .HasConversion(tagsConverter)
            .Metadata.SetValueComparer(tagsComparer);
    }
} 
