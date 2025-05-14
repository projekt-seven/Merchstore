using MerchStore.Application.Services.Interfaces;

namespace MerchStore.WebUI.Models.Api.Minimal;

/// <summary>
/// Response model for the Minimal API product endpoints.
/// </summary>
public class MinimalProductResponse
{
    /// <summary>
    /// The unique identifier of the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A description of the product.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The price amount of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The currency code for the price (e.g., "SEK", "USD").
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// The URL to the product image, if available.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The current stock quantity of the product.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Indicates whether the product is currently in stock.
    /// </summary>
    public bool InStock => StockQuantity > 0;

    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();

    private static async Task<IResult> GetAllProducts(ICatalogService catalogService)
    {
        var products = await catalogService.GetAllProductsAsync();

        var response = products.Select(p => new MinimalProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price.Amount,
            StockQuantity = p.StockQuantity,
            Category = p.Category,
            Tags = p.Tags // Map the Tags property
        }).ToList();

        return Results.Ok(response);
    }
}