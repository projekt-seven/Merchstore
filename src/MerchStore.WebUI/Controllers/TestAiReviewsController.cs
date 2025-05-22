using Microsoft.AspNetCore.Mvc;
using MerchStore.Infrastructure.ExternalServices.Reviews.Clients;
using MerchStore.Application.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MerchStore.WebUI.Controllers;

[ApiController]
[Route("test-ai")]
public class TestAiReviewsController : ControllerBase
{
    private readonly AiReviewsClient _client;
    private readonly ICatalogService _catalogService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TestAiReviewsController(
        AiReviewsClient client,
        ICatalogService catalogService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _client = client;
        _catalogService = catalogService;
        _httpClient = httpClientFactory.CreateClient("AiReviewsHttpClient");
        _configuration = configuration;
    }

    // 🔐 Logga in mot AI Reviews och hämta Bearer-token
    private async Task<string> GetBearerTokenAsync()
    {
        var username = Environment.GetEnvironmentVariable("AI_API_USERNAME") 
                    ?? _configuration["AiReviews:Auth:Username"];

        var password = Environment.GetEnvironmentVariable("AI_API_PASSWORD") 
                    ?? _configuration["AiReviews:Auth:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("AI API credentials are missing.");

        var loginPayload = new
        {
            username = username,
            password = password,
            authType = "password"
        };

        var loginResponse = await _httpClient.PostAsJsonAsync("https://aireviews.drillbi.se/auth/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("token").GetString();
    }

    /// <summary>
    /// Registrerar alla produkter från databasen hos AI Reviews
    /// </summary>
    [HttpPost("seed-products")]
    public async Task<IActionResult> SeedProductsToAiReviews()
    {
        var products = await _catalogService.GetAllProductsAsync();
        var results = new List<object>();
        string token;

        try
        {
            token = await GetBearerTokenAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Login failed: {ex.Message}" });
        }

        foreach (var product in products)
        {
            try
            {
                var payload = new
                {
                    mode = "withDetails",
                    productId = product.Id.ToString(),
                    productName = product.Name,
                    category = product.Category,
                    tags = product.Tags.Select(t => t.ToLower()).ToArray()
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "http://161.97.151.105:8081/product")
                {
                    Content = JsonContent.Create(payload)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                results.Add(new
                {
                    productId = product.Id,
                    status = "OK",
                    response = content
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    productId = product.Id,
                    status = "FAILED",
                    error = ex.Message
                });
            }
        }

        return Ok(results);
    }
}
