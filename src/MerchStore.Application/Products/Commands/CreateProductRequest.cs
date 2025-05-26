
using System.ComponentModel.DataAnnotations;
namespace MerchStore.Application.Products.Commands;

public class CreateProductRequest
{
    public Guid Id { get; set; }  // Används vid edit

    [Required(ErrorMessage = "Produktnamn är obligatoriskt")]
    [StringLength(100, ErrorMessage = "Namnet får vara max 100 tecken")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning är obligatoriskt")]
    [StringLength(500, ErrorMessage = "Beskrivningen får vara max 500 tecken")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 100000, ErrorMessage = "Pris måste vara ett positivt belopp")]
    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "Lagersaldo måste vara 0 eller högre")]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "Kategori är obligatoriskt")]
    [StringLength(100, ErrorMessage = "Kategorin får vara max 100 tecken")]
    public string Category { get; set; } = string.Empty;

    [Url(ErrorMessage = "Ogiltig bildlänk")]
    public string? ImageUrl { get; set; }

    public List<string>? Tags { get; set; }
}
	