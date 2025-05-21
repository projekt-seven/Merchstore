using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MerchStore.Infrastructure.ExternalServices.Reviews.Clients
{
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
                Console.WriteLine("❌ Login failed:");
                Console.WriteLine(body);
                return string.Empty;
            }

            var jsonDoc = JsonDocument.Parse(body);
            var token = jsonDoc.RootElement.GetProperty("token").GetString();
            Console.WriteLine("✅ Token received");
            return token;
        }

        public async Task<string> RegisterProductAsync(string productId, string name, string category, string[] tags)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return "Token retrieval failed. Cannot proceed.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var productData = new
            {
                mode = "withDetails",
                productId = productId,
                productName = name,
                category = category,
                tags = tags
            };

            var json = JsonSerializer.Serialize(productData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://aireviews.drillbi.se/product";

            Console.WriteLine("=== AI Reviews Request ===");
            Console.WriteLine($"URL: {url}");
            Console.WriteLine($"Token: {token.Substring(0, 15)}...");
            Console.WriteLine($"Body: {json}");

            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine("=== AI Reviews Response ===");
            Console.WriteLine($"Status: {(int)response.StatusCode}");
            Console.WriteLine($"Response: {body}");

            return body;
        }

        public async Task<string> GetProductDataAsync(string productId)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return "Token retrieval failed. Cannot proceed.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"https://aireviews.drillbi.se/product?productId={productId}";

            Console.WriteLine("=== AI Reviews GET Request ===");
            Console.WriteLine($"URL: {url}");
            Console.WriteLine($"Token: {token.Substring(0, 15)}...");

            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine("=== AI Reviews GET Response ===");
            Console.WriteLine($"Status: {(int)response.StatusCode}");
            Console.WriteLine($"Response: {body}");

            return body;
        }
    }
}
