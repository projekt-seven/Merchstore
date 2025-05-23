using Xunit;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.UnitTests.Entities;

public class ProductTests
{
    // Helper method to create a valid product for testing
    private Product CreateValidProduct()
    {
        return new Product(
            "Test Product",
            "Test Description",
            new Uri("https://example.com/image.jpg"),
            new Money(19.99m, "USD"),
            10,
            "Test Category");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesProduct()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act
        var product = new Product(name, description, imageUrl, price, stockQuantity, category);

        // Assert
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
        Assert.Equal(imageUrl, product.ImageUrl);
        Assert.Equal(price, product.Price);
        Assert.Equal(stockQuantity, product.StockQuantity);
        Assert.Equal(category, product.Category);
        Assert.NotEqual(Guid.Empty, product.Id);
    }

    [Theory]
    [InlineData("", "Test Description", "name")]
    [InlineData(null, "Test Description", "name")]
    [InlineData("Test Product", "", "description")]
    [InlineData("Test Product", null, "description")]
    public void Constructor_WithInvalidNameOrDescription_ThrowsArgumentException(string? name, string? description, string paramName)
    {
        // Arrange
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name!, description!, imageUrl, price, stockQuantity, category));

        Assert.Equal(paramName, exception.ParamName);
    }

    [Theory]
    [InlineData("A very long product name that exceeds the maximum allowed length of 100 characters which is meant to test validation logic", "Test Description", "name")]
    [InlineData("Test Product", "A very long product description that exceeds the maximum allowed length. It goes on and on with unnecessary details and filler content just to make sure we hit the 500 character limit that we've set for our validation logic. It keeps going with more and more text that doesn't really add any value but just takes up space to ensure we exceed the limit. We're adding even more text here to make absolutely certain that this description is too long for our product entity. This should definitely trigger the validation logic that checks for description length and throw an appropriate exception with the correct parameter name to help developers identify and fix the issue quickly.", "description")]
    public void Constructor_WithTooLongNameOrDescription_ThrowsArgumentException(string name, string description, string paramName)
    {
        // Arrange
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));

        Assert.Equal(paramName, exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullPrice_ThrowsArgumentNullException()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        Money price = null!; // Simulating null price
        int stockQuantity = 10;
        string category = "Test Category";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));
    }

    [Fact]
    public void Constructor_WithNegativeStockQuantity_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = -1;
        string category = "Test Category";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));

        Assert.Equal("stockQuantity", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidImageUrl_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("file:///C:/invalid/path.txt");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));

        Assert.Contains("URL must use HTTP or HTTPS", exception.Message);
    }

    [Fact]
    public void Constructor_WithEmptyCategory_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));

        Assert.Equal("category", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullCategory_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));

        Assert.Equal("category", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesProduct()
    {
        // Arrange
        var product = CreateValidProduct();
        string newName = "Updated Product";
        string newDescription = "Updated Description";
        var newImageUrl = new Uri("https://example.com/new-image.jpg");
        string newCategory = "Clothing";

        // Act
        product.UpdateDetails(newName, newDescription, newImageUrl, newCategory);

        // Assert
        Assert.Equal(newName, product.Name);
        Assert.Equal(newDescription, product.Description);
        Assert.Equal(newImageUrl, product.ImageUrl);
        Assert.Equal(newCategory, product.Category);
    }

    [Theory]
    [InlineData("", "Updated Description", "name")]
    [InlineData(null, "Updated Description", "name")]
    [InlineData("Updated Product", "", "description")]
    [InlineData("Updated Product", null, "description")]
    public void UpdateDetails_WithInvalidParameters_ThrowsArgumentException(string? newName, string? newDescription, string paramName)
    {
        // Arrange
        var product = CreateValidProduct();
        var newImageUrl = new Uri("https://example.com/new-image.jpg");
        string newCategory = "Clothing";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            product.UpdateDetails(newName!, newDescription!, newImageUrl, newCategory));

        Assert.Equal(paramName, exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithEmptyCategory_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();
        string newName = "Updated Product";
        string newDescription = "Updated Description";
        var newImageUrl = new Uri("https://example.com/new-image.jpg");
        string newCategory = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            product.UpdateDetails(newName, newDescription, newImageUrl, newCategory));

        Assert.Equal("category", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithNullCategory_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();
        string newName = "Updated Product";
        string newDescription = "Updated Description";
        var newImageUrl = new Uri("https://example.com/new-image.jpg");
        string newCategory = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            product.UpdateDetails(newName, newDescription, newImageUrl, newCategory));

        Assert.Equal("category", exception.ParamName);
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_UpdatesPrice()
    {
        // Arrange
        var product = CreateValidProduct();
        var newPrice = new Money(29.99m, "USD");

        // Act
        product.UpdatePrice(newPrice);

        // Assert
        Assert.Equal(newPrice, product.Price);
    }

    [Fact]
    public void UpdatePrice_WithNullPrice_ThrowsArgumentNullException()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => product.UpdatePrice(null!));
    }

    [Fact]
    public void UpdateStock_WithValidQuantity_UpdatesStockQuantity()
    {
        // Arrange
        var product = CreateValidProduct();
        int newQuantity = 20;

        // Act
        product.UpdateStock(newQuantity);

        // Assert
        Assert.Equal(newQuantity, product.StockQuantity);
    }

    [Fact]
    public void UpdateStock_WithNegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();
        int newQuantity = -1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            product.UpdateStock(newQuantity));

        Assert.Equal("quantity", exception.ParamName);
    }

    [Fact]
    public void DecrementStock_WithValidQuantity_DecreasesStock()
    {
        // Arrange
        var product = CreateValidProduct();
        int initialStock = product.StockQuantity;
        int decrementAmount = 3;

        // Act
        bool result = product.DecrementStock(decrementAmount);

        // Assert
        Assert.True(result);
        Assert.Equal(initialStock - decrementAmount, product.StockQuantity);
    }

    [Fact]
    public void DecrementStock_WithDefaultQuantity_DecreasesByOne()
    {
        // Arrange
        var product = CreateValidProduct();
        int initialStock = product.StockQuantity;

        // Act
        bool result = product.DecrementStock();

        // Assert
        Assert.True(result);
        Assert.Equal(initialStock - 1, product.StockQuantity);
    }

    [Fact]
    public void DecrementStock_WithInsufficientStock_ReturnsFalse()
    {
        // Arrange
        var product = CreateValidProduct();
        int initialStock = product.StockQuantity;
        int decrementAmount = initialStock + 1;

        // Act
        bool result = product.DecrementStock(decrementAmount);

        // Assert
        Assert.False(result);
        Assert.Equal(initialStock, product.StockQuantity); // Stock should remain unchanged
    }

    [Fact]
    public void DecrementStock_WithNegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.DecrementStock(-1));
    }

    [Fact]
    public void IncrementStock_WithValidQuantity_IncreasesStock()
    {
        // Arrange
        var product = CreateValidProduct();
        int initialStock = product.StockQuantity;
        int incrementAmount = 5;

        // Act
        product.IncrementStock(incrementAmount);

        // Assert
        Assert.Equal(initialStock + incrementAmount, product.StockQuantity);
    }

    [Fact]
    public void IncrementStock_WithZeroQuantity_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.IncrementStock(0));
    }

    [Fact]
    public void IncrementStock_WithNegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.IncrementStock(-1));
    }

    // Test boundary for name length (exactly 100 characters)
    [Fact]
    public void Constructor_WithNameAtMaxLength_CreatesProduct()
    {
        // Arrange
        string name = new string('A', 100);
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act
        var product = new Product(name, description, imageUrl, price, stockQuantity, category);

        // Assert
        Assert.Equal(name, product.Name);
    }

    // Test boundary for description length (exactly 500 characters)
    [Fact]
    public void Constructor_WithDescriptionAtMaxLength_CreatesProduct()
    {
        // Arrange
        string name = "Test Product";
        string description = new string('A', 500);
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act
        var product = new Product(name, description, imageUrl, price, stockQuantity, category);

        // Assert
        Assert.Equal(description, product.Description);
    }

    // Test boundary for image URL length (exactly 2000 characters)
    [Fact]
    public void Constructor_WithImageUrlAtMaxLength_CreatesProduct()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        string longUrl = "https://example.com/" + new string('a', 1970) + ".jpg"; // Adjusted to fit within URI limits
        var imageUrl = new Uri(longUrl);
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act
        var product = new Product(name, description, imageUrl, price, stockQuantity, category);

        // Assert
        Assert.Equal(imageUrl, product.ImageUrl);
    }

    // Test invalid image URL extension
    [Fact]
    public void Constructor_WithInvalidImageUrlExtension_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.txt");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Product(name, description, imageUrl, price, stockQuantity, category));

        Assert.Contains("Image URL must point to a valid image file", exception.Message);
    }

    // Test decrementing stock to exactly 0
    [Fact]
    public void DecrementStock_ToZero_Succeeds()
    {
        // Arrange
        var product = CreateValidProduct();
        int initialStock = product.StockQuantity;

        // Act
        bool result = product.DecrementStock(initialStock);

        // Assert
        Assert.True(result);
        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void Constructor_WithValidTags_CreatesProduct()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";
        var tags = new List<string> { "Tag1", "Tag2", "Tag3" };

        // Act
        var product = new Product(name, description, imageUrl, price, stockQuantity, category, tags);

        // Assert
        Assert.Equal(tags, product.Tags);
    }

    [Fact]
    public void Constructor_WithNullTags_SetsEmptyTagsList()
    {
        // Arrange
        string name = "Test Product";
        string description = "Test Description";
        var imageUrl = new Uri("https://example.com/image.jpg");
        var price = new Money(19.99m, "USD");
        int stockQuantity = 10;
        string category = "Test Category";

        // Act
        var product = new Product(name, description, imageUrl, price, stockQuantity, category, null);

        // Assert
        Assert.NotNull(product.Tags);
        Assert.Empty(product.Tags);
    }

    [Fact]
    public void UpdateTags_WithValidTags_UpdatesTags()
    {
        // Arrange
        var product = CreateValidProduct();
        var newTags = new List<string> { "UpdatedTag1", "UpdatedTag2" };

        // Act
        product.UpdateTags(newTags);

        // Assert
        Assert.Equal(newTags, product.Tags);
    }

    [Fact]
    public void UpdateTags_WithNullTags_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.UpdateTags(null!));
    }

    [Fact]
    public void UpdateTags_WithEmptyOrWhitespaceTags_ThrowsArgumentException()
    {
        // Arrange
        var product = CreateValidProduct();
        var invalidTags = new List<string> { "ValidTag", " " };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.UpdateTags(invalidTags));
    }
}