namespace MerchStore.Application.Products.Queries;

public class ProductListItemDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockQuantity { get; set; }


}