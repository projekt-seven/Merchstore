using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MerchStore.WebUI.Controllers.Api.Integration
{
    /// <summary>
    /// Exponeras som ett integration-API anpassat för Java Integration-gruppen.
    /// Returnerar produkt + recensioner i ett aggregerat format.
    /// </summary>
    [ApiController]
    [Authorize(Policy = "ApiKeyPolicy")]
    public class ProductsIntegrationController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IReviewService _reviewService;

        public ProductsIntegrationController(
            ICatalogService catalogService,
            IReviewService reviewService)
        {
            _catalogService = catalogService;
            _reviewService = reviewService;
        }

        /// <summary>
        /// GET /products?productId=...
        /// Returnerar produktinformation + statistik + recensioner i Java-format
        /// </summary>
        [HttpGet("products")]
        public async Task<IActionResult> GetProductWithReviews([FromQuery] string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest(new { message = "Missing required query parameter: productId" });

            if (!Guid.TryParse(productId, out var productGuid))
                return BadRequest(new { message = "Invalid productId format. Must be a valid GUID." });

            var product = await _catalogService.GetProductByIdAsync(productGuid);
            if (product == null)
                return NotFound(new { message = $"Product with ID {productId} not found" });

            var reviews = await _reviewService.GetReviewsByProductIdAsync(productGuid);
            var average = await _reviewService.GetAverageRatingForProductAsync(productGuid);
            var count = await _reviewService.GetReviewCountForProductAsync(productGuid);
            var last = reviews
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault()
                ?.CreatedAt
                .ToString("yyyy-MM-dd");

            var response = new
            {
                product_id = product.Id,
                stats = new
                {
                    product_name = product.Name,
                    current_average = Math.Round(average, 2),
                    total_reviews = count,
                    last_review_date = last
                },
                reviews = reviews.Select(r => new
                {
                    date = r.CreatedAt.ToString("yyyy-MM-dd"),
                    name = r.CustomerName,
                    rating = r.Rating,
                    text = r.Content
                })
            };

            return Ok(response);
        }
    }
}