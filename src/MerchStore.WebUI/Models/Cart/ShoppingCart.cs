using System.Text.Json;

namespace MerchStore.WebUI.Models.Cart;

public class ShoppingCart
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    public decimal Total => Items.Sum(item => item.Price * item.Quantity);
    
    public int ItemCount => Items.Sum(item => item.Quantity);

    // Helper methods for cookie-based cart

    public static ShoppingCart GetFromCookie(HttpContext httpContext)
    {
        var cartCookie = httpContext.Request.Cookies["shopping_cart"];
        if (string.IsNullOrEmpty(cartCookie))
        {
            return new ShoppingCart();
        }

        try
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cartCookie) ?? new ShoppingCart();
        }
        catch
        {
            // If deserialize fails, return a new empty cart
            return new ShoppingCart();
        }
    }

    public void SaveToCookie(HttpContext httpContext)
    {
        var cartJson = JsonSerializer.Serialize(this);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(7),
            IsEssential = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        };

        httpContext.Response.Cookies.Delete("shopping_cart");
        httpContext.Response.Cookies.Append("shopping_cart", cartJson, cookieOptions);
    }

    public void AddItem(Guid productId, string name, decimal price, string? imageUrl, int quantity = 1)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new CartItem
            {
                ProductId = productId,
                Name = name,
                Price = price,
                Quantity = quantity,
                ImageUrl = imageUrl
            });
        }
    }

    public void RemoveItem(Guid productId)
    {
        var itemToRemove = Items.FirstOrDefault(i => i.ProductId == productId);
        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
        }
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                RemoveItem(productId);
            }
            else
            {
                item.Quantity = quantity;
            }
        }
    }

    public void Clear()
    {
        Items.Clear();
    }
}
