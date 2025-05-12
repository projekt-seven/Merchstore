using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.Api.Minimal;

namespace MerchStore.WebUI.Endpoints;

public static class MinimalProductEndpoints
{
    public static WebApplication MapMinimalProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/minimal/products")
            .WithTags("Minimal Products API")
            .WithOpenApi();

        group.RequireAuthorization("ApiKeyPolicy");

        group.MapGet("/", GetAllProducts)
            .WithName("GetAllProductsMinimal")
            .WithDescription("Gets all available products")
            .Produces<List<MinimalProductResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetProductById)
            .WithName("GetProductByIdMinimal")
            .WithDescription("Gets a specific product by its unique identifier")
            .Produces<MinimalProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetAllProducts(ICatalogService catalogService)
    {
        try
        {
            var products = await catalogService.GetAllProductsAsync();

            var response = products.Select(p => new MinimalProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.Amount,
                Currency = p.Price.Currency,
                ImageUrl = p.ImageUrl?.ToString(),
                StockQuantity = p.StockQuantity,
                Tags = p.Tags
            }).ToList();

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ FEL i GetAllProducts: " + ex.Message);
            return Results.Problem($"An error occurred while retrieving products: {ex.Message}");
        }
    }

    private static async Task<IResult> GetProductById(Guid id, ICatalogService catalogService)
    {
        try
        {
            var product = await catalogService.GetProductByIdAsync(id);

            if (product is null)
            {
                return Results.NotFound($"Product with ID {id} not found");
            }

            var response = new MinimalProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                ImageUrl = product.ImageUrl?.ToString(),
                StockQuantity = product.StockQuantity,
                Tags = product.Tags
            };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ FEL i GetProductById: " + ex.Message);
            return Results.Problem($"An error occurred while retrieving the product: {ex.Message}");
        }
    }
}
