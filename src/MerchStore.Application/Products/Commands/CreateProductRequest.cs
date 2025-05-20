namespace MerchStore.Application.Products.Commands
{
	public class CreateProductRequest
	{
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }   // decimal → blir Money i domen
		public int StockQuantity { get; set; } // OBS: ändrat från Stock
		public string Category { get; set; } = string.Empty;
		public string? ImageUrl { get; set; } // nullable
		public List<string>? Tags { get; set; } // valfritt
	}
}