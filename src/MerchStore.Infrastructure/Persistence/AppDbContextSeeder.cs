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

        var products = new List<Product>
        {
            new Product(
                "White T-Shirt",
                "A white cotton t-shirt featuring the North Waddle logo on the front.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(249.00m),
                60,
                "t-shirts"),

            new Product(
                "Black Sweater",
                "A black cozy sweater with the North Waddle logo printed on the chest.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(499.00m),
                40,
                "sweaters"),

            new Product(
                "2025 Calendar",
                "A 12-month calendar for 2025 featuring custom North Waddle design and branding.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(129.00m),
                75,
                "calendars"),

            new Product(
                "Logo Coasters (Set of 4)",
                "Set of 4 white background coasters with the North Waddle logo printed on each.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(99.00m),
                120,
                "home"),

            new Product(
                "White Tote Bag",
                "A durable white tote bag featuring the North Waddle logo, great for everyday use.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(149.00m),
                90,
                "bags"),

            new Product(
                "Water Bottle",
                "A sleek white water bottle printed with the North Waddle logo, keeps drinks cold or hot.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(199.00m),
                70,
                "bottles"),

            new Product(
                "North Waddle Sticker Pack",
                "Fun and colorful stickers featuring the North Waddle brand and mascot.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(59.00m),
                150,
                "stickers"),

            new Product(
                "North Waddle Notebook",
                "A stylish and practical notebook with North Waddle branding for your notes and ideas.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(119.00m),
                80,
                "stationery")
        };
            // Add sample products
            /* var products = new List<Product>
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
 */
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