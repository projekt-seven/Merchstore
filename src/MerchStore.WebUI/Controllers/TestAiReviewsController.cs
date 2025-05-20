using Microsoft.AspNetCore.Mvc;
using MerchStore.Infrastructure.ExternalServices.Reviews.Clients;

namespace MerchStore.WebUI.Controllers;

[ApiController]
[Route("test-ai")]
public class TestAiReviewsController : ControllerBase
{
    private readonly AiReviewsClient _client;

    public TestAiReviewsController(AiReviewsClient client)
    {
        _client = client;
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetFromAiReviews(string productId)
    {
        try
        {
            var data = await _client.GetProductDataAsync(productId);
            return Content(data, "application/json"); // returnera rå JSON direkt
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
