using Microsoft.AspNetCore.Mvc;
using MerchStore.WebUI.Models.Cart;

namespace MerchStore.WebUI.ViewComponents;

public class CartCountViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cart = ShoppingCart.GetFromCookie(HttpContext);
        return View(cart.ItemCount);
    }
}
