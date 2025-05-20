using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MerchStore.Infrastructure.ExternalServices.Reviews.Clients;

public class AiReviewsClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AiReviewsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        var loginData = new
        {
            username = "test_user",
            password = "secret123",
            authType = "password"
        };

        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://aireviews.drillbi.se/auth/login", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return body; // Returnera API-svaret direkt även vid fel
        }

        var jsonDoc = JsonDocument.Parse(body);
        return jsonDoc.RootElement.GetProperty("token").GetString();
    }

    public async Task<string> GetProductDataAsync(string productId)
    {
        var token = await GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"https://aireviews.drillbi.se/products?productId={productId}");
        var body = await response.Content.ReadAsStringAsync();

        return body; // Returnera alltid svaret – även vid 500
    }
}
