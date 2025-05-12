using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using System.Text.Json;

namespace MerchStore.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Money converter
        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount,
            v => Money.FromSEK(v)
        );

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey("CustomerId")
            .IsRequired();

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasConversion(moneyConverter);

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
