using Microsoft.AspNetCore.Mvc;
using MerchStore.Infrastructure.ExternalServices.Reviews.Clients;
using MerchStore.Application.Services.Interfaces;

namespace MerchStore.WebUI.Controllers;

[ApiController]
[Route("test-ai")]
public class TestAiReviewsController : ControllerBase
{
    private readonly AiReviewsClient _client;
    private readonly ICatalogService _catalogService;

    public TestAiReviewsController(AiReviewsClient client, ICatalogService catalogService)
    {
        _client = client;
        _catalogService = catalogService;
    }

    /// <summary>
    /// Hämtar produkt + recensioner från AI Reviews
    /// </summary>
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetFromAiReviews(string productId)
    {
        try
        {
            var data = await _client.GetProductDataAsync(productId);
            return Content(data, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Registrerar en hårdkodad testprodukt i AI Reviews
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterProductForAiReviews()
    {
        try
        {
            var result = await _client.RegisterProductAsync(
                productId: "T888",
                name: "Simple Shirt",
                category: "t-shirts",
                tags: new[] { "basic", "white" }
            );

            return Content(result, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Registrerar alla produkter från databasen hos AI Reviews
    /// </summary>
    [HttpPost("seed-products")]
    public async Task<IActionResult> SeedProductsToAiReviews()
    {
        var products = await _catalogService.GetAllProductsAsync();
        var results = new List<object>();

        foreach (var product in products)
        {
            try
            {
                var result = await _client.RegisterProductAsync(
                    product.Id.ToString(),
                    product.Name,
                    product.Category,
                    product.Tags.Select(t => t.ToLower()).ToArray()
                );

                results.Add(new { productId = product.Id, status = "OK", response = result });
            }
            catch (Exception ex)
            {
                results.Add(new { productId = product.Id, status = "FAILED", error = ex.Message });
            }
        }

        return Ok(results);
    }
}
