using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor that accepts the context and a logger
    /// </summary>
    /// <param name="context">The database context to seed</param>
    /// <param name="logger">The logger for logging seed operations</param>
    public AppDbContextSeeder(AppDbContext context, ILogger<AppDbContextSeeder> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
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

            // Seed orders if none exist
            await SeedOrdersAsync();

            await SeedUsersAsync();
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
                "t-shirts",
                new List<string> { "North Waddle", "t-shirt", "white", "cotton" }),

            new Product(
                "Black Sweater",
                "A black cozy sweater with the North Waddle logo printed on the chest.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(499.00m),
                40,
                "sweaters",
                new List<string> { "North Waddle", "sweater", "black", "cotton" }),

            new Product(
                "2025 Calendar",
                "A 12-month calendar for 2025 featuring custom North Waddle design and branding.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(129.00m),
                75,
                "calendars",
                new List<string> { "North Waddle", "calendar", "2025", "penguin" }),

            new Product(
                "Logo Coasters (Set of 4)",
                "Set of 4 white background coasters with the North Waddle logo printed on each.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(99.00m),
                120,
                "home",
                new List<string> { "North Waddle", "coasters", "white background", "penguin" }),

            new Product(
                "White Tote Bag",
                "A durable white tote bag featuring the North Waddle logo, great for everyday use.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(149.00m),
                90,
                "bags",
                new List<string> { "North Waddle", "tote bag", "white", "penguin" }),

            new Product(
                "Water Bottle",
                "A sleek white water bottle printed with the North Waddle logo, keeps drinks cold or hot.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(199.00m),
                70,
                "bottles",
                new List<string> { "North Waddle", "water bottle", "white", "penguin" }),

            new Product(
                "North Waddle Sticker Pack",
                "Fun and colorful stickers featuring the North Waddle brand and mascot.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(59.00m),
                150,
                "stickers",
                new List<string> { "North Waddle", "stickers", "white", "penguin" }),

            new Product(
                "North Waddle Notebook",
                "A stylish and practical notebook with North Waddle branding for your notes and ideas.",
                new Uri("https://merchstore202503311226.blob.core.windows.net/images/tshirt.png"),
                Money.FromSEK(119.00m),
                80,
                "stationery",
                new List<string> { "North Waddle", "notebook", "practical", "penguin" })
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

    private async Task SeedOrdersAsync()
    {
        // Check if we already have orders (to avoid duplicate seeding)
        if (!await _context.Orders.AnyAsync())
        {
            _logger.LogInformation("Seeding orders...");

            // Retrieve the seeded products
            var products = await _context.Products.ToListAsync();

            if (products.Count < 2)
            {
                _logger.LogWarning("Not enough products available to seed orders.");
                return;
            }

            // Create customers
            var customers = new List<Customer>
            {
                new Customer("Johan", "Svensson", "johan.svensson@example.com", "0701234567", "Storgatan 1", "Stockholm", "11122"),
                new Customer("Anna", "Karlsson", "anna.karlsson@example.com", "0707654321", "Långgatan 5", "Göteborg", "41123"),
                new Customer("Erik", "Johansson", "erik.johansson@example.com", "0709876543", "Kyrkogatan 3", "Malmö", "21124"),
                new Customer("Maria", "Andersson", "maria.andersson@example.com", "0706543210", "Västra Hamngatan 7", "Uppsala", "75125"),
                new Customer("Lars", "Nilsson", "lars.nilsson@example.com", "0703210987", "Östra Långgatan 9", "Västerås", "72126"),
                new Customer("Karin", "Bergström", "karin.bergstrom@example.com", "0704321098", "Kungsgatan 11", "Örebro", "70127"),
                new Customer("Olof", "Lindgren", "olof.lindgren@example.com", "0705432109", "Drottninggatan 13", "Linköping", "58128"),
                new Customer("Sofia", "Eriksson", "sofia.eriksson@example.com", "0706543211", "Nygatan 15", "Helsingborg", "25129"),
                new Customer("Gustav", "Persson", "gustav.persson@example.com", "0707654322", "Södra Vägen 17", "Jönköping", "55130"),
                new Customer("Elin", "Olsson", "elin.olsson@example.com", "0708765432", "Norra Långgatan 19", "Lund", "22131")
            };

            // Create orders for each customer
            var orders = customers.Select(customer => new Order(customer)).ToList();

            // Use reflection to set the OrderDate property
            var orderDateProperty = typeof(Order).GetProperty("OrderDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (orderDateProperty != null)
            {
                orderDateProperty.SetValue(orders[1], DateTime.UtcNow.AddDays(-8));
                orderDateProperty.SetValue(orders[4], DateTime.UtcNow.AddDays(-2));
                orderDateProperty.SetValue(orders[6], DateTime.UtcNow);
                orderDateProperty.SetValue(orders[9], DateTime.UtcNow.AddDays(-7));
            }

            // Update the status using existing methods
            orders[0].MarkAsCompleted();
            orders[2].CancelOrder();
            orders[3].MarkAsCompleted();
            orders[5].MarkAsCompleted();
            orders[7].CancelOrder();
            orders[8].MarkAsCompleted();

            foreach (var order in orders)
            {
                order.AddItem(new OrderItem(products[0].Id, 2, products[0].Price)); // First product
                order.AddItem(new OrderItem(products[1].Id, 1, products[1].Price)); // Second product
            }

            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order seeding completed successfully.");
        }
        else
        {
            _logger.LogInformation("Database already contains orders. Skipping order seed.");
        }
    }

    private async Task SeedUsersAsync()
    {
        if (!await _context.Users.AnyAsync())
        {
            //För dev
            /*
            var adminUsername = _configuration["AdminUser:Username"];
            var adminPassword = _configuration["AdminUser:Password"];
            var adminEmail = _configuration["AdminUser:Email"];
            var adminRole = _configuration["AdminUser:AdminRole"];*/

            //För prod
            var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? _configuration["AdminUser:Username"];
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? _configuration["AdminUser:Password"];
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? _configuration["AdminUser:Email"];
            var adminRole = Environment.GetEnvironmentVariable("ADMIN_ROLE") ?? _configuration["AdminUser:AdminRole"];


            if (string.IsNullOrWhiteSpace(adminUsername) ||
                string.IsNullOrWhiteSpace(adminPassword) ||
                string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminRole))
            {
                _logger.LogError("One or more required environment variables for admin user are missing.");
                throw new InvalidOperationException("Missing required environment variables for admin user.");
            }

            _logger.LogInformation("Seeding admin user...");

            var hashedPassword = HashPassword(adminPassword);

            _context.Users.Add(new User(adminUsername, hashedPassword, adminEmail, adminRole));
            await _context.SaveChangesAsync();
        }
    }

    private string HashPassword(string password)
    {
        // Use a secure hashing algorithm like BCrypt or PBKDF2
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}