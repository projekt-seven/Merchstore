using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MerchStore.Application.DTOs;
using MerchStore.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

public class AiReviewService : IAiReviewService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AiReviewService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AiReviewResponse?> GetReviewAsync(Guid productId)
    {
        var token = await GetBearerTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://aireviews.drillbi.se/product/{productId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AiReviewResponse>();
    }

    private async Task<string> GetBearerTokenAsync()
    {
        var username = Environment.GetEnvironmentVariable("AI_API_USERNAME")
                       ?? _configuration["AiReviews:Auth:Username"];
        var password = Environment.GetEnvironmentVariable("AI_API_PASSWORD")
                       ?? _configuration["AiReviews:Auth:Password"];

        var loginPayload = new
        {
            username,
            password,
            authType = "password"
        };

        var loginResponse = await _httpClient.PostAsJsonAsync("https://aireviews.drillbi.se/auth/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("token").GetString();
    }
}
