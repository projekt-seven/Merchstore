using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.Catalog;
using Microsoft.AspNetCore.Hosting;
using MerchStore.Application.DTOs;

namespace MerchStore.WebUI.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly IAiReviewService _aiReviewService;

    public CatalogController(ICatalogService catalogService, IAiReviewService aiReviewService)
    {
        _catalogService = catalogService;
        _aiReviewService = aiReviewService;
    }

    // GET: Catalog
    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await _catalogService.GetAllProductsAsync();

            var productViewModels = products.Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                TruncatedDescription = p.Description.Length > 100
                    ? p.Description.Substring(0, 97) + "..."
                    : p.Description,
                FormattedPrice = p.Price.ToString(),
                PriceAmount = p.Price.Amount,
                ImageUrl = p.ImageUrl?.ToString(),
                StockQuantity = p.StockQuantity
            }).ToList();

            var viewModel = new ProductCatalogViewModel
            {
                FeaturedProducts = productViewModels
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ProductCatalog: {ex.Message}");
            ViewBag.ErrorMessage = "An error occurred while loading products. Please try again later.";
            return View("Error");
        }
    }

    // GET: Store/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var product = await _catalogService.GetProductByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            var aiReviews = await _aiReviewService.GetReviewAsync(id);

            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                FormattedPrice = product.Price.ToString(),
                PriceAmount = product.Price.Amount,
                ImageUrl = product.ImageUrl?.ToString(),
                StockQuantity = product.StockQuantity,
                Reviews = aiReviews
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CatalogController.Details: {ex.Message}");
            ViewBag.ErrorMessage = "An error occurred while loading the product. Please try again later.";
            return View("Error");
        }
    }

    /* 🟢 Mockfunktion för dev-miljö
    private AiReviewResponse GetMockReviews()
    {
        return new AiReviewResponse
        {
            Stats = new AiReviewStats
            {
                CurrentAverage = 4.8,
                TotalReviews = 2,
                LastReviewDate = DateTime.UtcNow
            },
            Reviews = new List<AiSingleReview>
            {
                new AiSingleReview
                {
                    Name = "Mock Tester",
                    Date = DateTime.UtcNow,
                    Rating = 5,
                    Text = "Mocked review in development mode!"
                },
                new AiSingleReview
                {
                    Name = "Mock Reviewer 2",
                    Date = DateTime.UtcNow.AddDays(-2),
                    Rating = 4,
                    Text = "Second mocked review, looks great."
                }
            }
        };
    }*/
}
