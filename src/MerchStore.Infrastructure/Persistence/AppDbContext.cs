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

        // Apply IEntityTypeConfiguration configs
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ValueConverter for Money → decimal
        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount,
            v => Money.FromSEK(v)
        );

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasConversion(moneyConverter);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasConversion(moneyConverter);

        // User mapping
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

        // Product.Tags → List<string> JSON mapping
        var tagsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null!) ?? new()
        );

        var tagsComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c == null ? new List<string>() : c.ToList()
        );

        modelBuilder.Entity<Product>()
            .Property(p => p.Tags)
            .HasConversion(tagsConverter)
            .Metadata.SetValueComparer(tagsComparer);
    }
}
