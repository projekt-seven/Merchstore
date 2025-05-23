using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.Cart;

namespace MerchStore.WebUI.Controllers;

public class CartController : Controller
{
    private readonly ICatalogService _catalogService;

    public CartController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    // GET: Cart
    public IActionResult Index()
    {
        var cart = ShoppingCart.GetFromCookie(HttpContext);
        var viewModel = new ShoppingCartViewModel
        {
            Items = cart.Items,
            Total = cart.Total,
            ItemCount = cart.ItemCount
        };

        return View(viewModel);
    }    // POST: Cart/AddItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(Guid productId, int quantity = 1)
    {
        // Get product details from catalog service
        var product = await _catalogService.GetProductByIdAsync(productId);
        
        if (product == null)
        {
            return Json(new { success = false, message = "Product not found" });
        }

        // Get current cart from cookie
        var cart = ShoppingCart.GetFromCookie(HttpContext);
        
        // Add product to cart
        cart.AddItem(
            productId,
            product.Name,
            product.Price.Amount,
            product.ImageUrl?.ToString(),
            quantity
        );
        
        // Save updated cart to cookie
        cart.SaveToCookie(HttpContext);
        
        // Return success with updated cart count
        return Json(new { success = true, count = cart.ItemCount });
    }
      // POST: Cart/RemoveItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(Guid productId)
    {
        var cart = ShoppingCart.GetFromCookie(HttpContext);
        cart.RemoveItem(productId);
        cart.SaveToCookie(HttpContext);
        
        return RedirectToAction(nameof(Index));
    }
    
    // POST: Cart/UpdateQuantity
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(Guid productId, int quantity)
    {
        var cart = ShoppingCart.GetFromCookie(HttpContext);
        cart.UpdateItemQuantity(productId, quantity);
        cart.SaveToCookie(HttpContext);
        
        return RedirectToAction(nameof(Index));
    }
    
    // POST: Cart/Clear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        var cart = ShoppingCart.GetFromCookie(HttpContext);
        cart.Clear();
        cart.SaveToCookie(HttpContext);
        
        return RedirectToAction(nameof(Index));
    }
}
