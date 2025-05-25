namespace MerchStore.Application.Products.DTOs;

public class ProductDetailsDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockQuantity { get; set; }
	public string Category { get; set; } = string.Empty;
	public string? ImageUrl { get; set; }
	public List<string> Tags { get; set; } = new();
}