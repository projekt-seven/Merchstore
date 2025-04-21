using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Infrastructure.Persistence;

/// <summary>
/// Class for seeding the database with initial data.
/// This is useful for development, testing, and demos.
/// </summary>
public class AppDbContextSeeder
{
    private readonly ILogger<AppDbContextSeeder> _logger;
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor that accepts the context and a logger
    /// </summary>
    /// <param name="context">The database context to seed</param>
    /// <param name="logger">The logger for logging seed operations</param>
    public AppDbContextSeeder(AppDbContext context, ILogger<AppDbContextSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    public virtual async Task SeedAsync()
    {
        try
        {
            // Ensure the database is created (only needed for in-memory database)
            // For SQL Server, you would use migrations instead
            await _context.Database.EnsureCreatedAsync();

            // Seed products if none exist
            await SeedProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    /// <summary>
    /// Seeds the database with sample products
    /// </summary>
    private async Task SeedProductsAsync()
    {
        // Check if we already have products (to avoid duplicate seeding)
        if (!await _context.Products.AnyAsync())
        {
            _logger.LogInformation("Seeding products...");

            // Add sample products
            var products = new List<Product>
            {
                new Product(
                    "Conference T-Shirt",
                    "A comfortable cotton t-shirt with the conference logo.",
                    new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                    Money.FromSEK(249.99m),
                    50,
                    "t-shirts"),

                new Product(
                    "Developer Mug",
                    "A ceramic mug with a funny programming joke.",
                    new Uri("https://merchstore202503311226.blob.core.windows.net/images/mug.png"),
                    Money.FromSEK(149.50m),
                    100,
                    "mugs"),

                new Product(
                    "Laptop Sticker Pack",
                    "A set of 5 programming language stickers for your laptop.",
                    new Uri("https://merchstore202503311226.blob.core.windows.net/images/stickers.png"),
                    Money.FromSEK(79.99m),
                    200,
                    "stickers"),

                new Product(
                    "Branded Hoodie",
                    "A warm hoodie with the company logo, perfect for cold offices.",
                    new Uri("https://merchstore202503311226.blob.core.windows.net/images/hoodie.png"),
                    Money.FromSEK(499.99m),
                    25,
                    "hoodies"),
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product seeding completed successfully.");
        }
        else
        {
            _logger.LogInformation("Database already contains products. Skipping product seed.");
        }
    }
}