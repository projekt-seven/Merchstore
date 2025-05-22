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

    public TestAiReviewsController(
        AiReviewsClient client,
        ICatalogService catalogService,
        IHttpClientFactory httpClientFactory
        IConfiguration configuration)
    {
        _client = client;
        _catalogService = catalogService;
        _httpClient = httpClientFactory.CreateClient("AiReviewsHttpClient");
    }

    // 🔐 Logga in mot AI Reviews och hämta Bearer-token
    private async Task<string> GetBearerTokenAsync()
    {
        var loginPayload = new
        {
            username = "hugo",        // <-- byt till ditt användarnamn
            password = "secret123",        // <-- och lösenord
            authType = "password"
        };

        var loginResponse = await _httpClient.PostAsJsonAsync("http://161.97.151.105:8081/auth/login", loginPayload);
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
