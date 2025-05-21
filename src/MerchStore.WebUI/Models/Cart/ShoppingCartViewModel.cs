namespace MerchStore.WebUI.Models.Cart;

public class ShoppingCartViewModel
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal Total { get; set; }
    public int ItemCount { get; set; }
}
