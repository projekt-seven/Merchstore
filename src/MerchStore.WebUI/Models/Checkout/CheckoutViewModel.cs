using System.ComponentModel.DataAnnotations;

namespace MerchStore.WebUI.Models.Checkout
{
    public class CheckoutViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string PhoneNumber { get; set; }        // Order summary
        public List<Cart.CartItem> Items { get; set; } = new List<Cart.CartItem>();
        public decimal Total { get; set; }
    }
}
