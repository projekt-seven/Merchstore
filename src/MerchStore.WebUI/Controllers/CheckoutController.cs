using Microsoft.AspNetCore.Mvc;
using MerchStore.WebUI.Models.Checkout;
using MerchStore.WebUI.Models.Cart;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Infrastructure.Persistence;
using MerchStore.Application.Services.Interfaces;

namespace MerchStore.WebUI.Controllers
{    public class CheckoutController : Controller
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly AppDbContext _dbContext;
        private readonly ICatalogService _catalogService;
        
        public CheckoutController(IRepositoryManager repositoryManager, AppDbContext dbContext, ICatalogService catalogService)
        {
            _repositoryManager = repositoryManager;
            _dbContext = dbContext;
            _catalogService = catalogService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = ShoppingCart.GetFromCookie(HttpContext);
            var model = new CheckoutViewModel
            {
                Items = cart.Items,
                Total = cart.Total
            };
            return View(model);
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            // Get the current cart from the cookie
            var cart = ShoppingCart.GetFromCookie(HttpContext);
            
            // Always set cart items into the model since they aren't passed with the form
            model.Items = cart.Items;
            model.Total = cart.Total;
            
            // Check if the cart is empty
            if (cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty. Please add items before checkout.");
                return View("Index", model);
            }
            
            // Check the model state (customer information fields)
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Map CheckoutViewModel to Domain Customer
            var customer = new Customer(
                model.FirstName,
                model.LastName,
                model.Email,
                model.PhoneNumber,
                model.Address,
                model.City,
                model.PostalCode
            );

            // Save customer to DB if not exists
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            // Create Order and add items
            var order = new Order(customer);
            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem(
                    item.ProductId,
                    item.Quantity,
                    Money.FromSEK(item.Price)
                );
                order.AddItem(orderItem);
            }

            await _repositoryManager.OrderRepository.AddAsync(order);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            // Optionally clear cart after order
            cart.Clear();
            cart.SaveToCookie(HttpContext);

            // Pass order info to receipt view (map to viewmodel as needed)
            var receiptModel = new ReceiptViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                CustomerName = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = $"{model.Address}, {model.City}, {model.PostalCode}, {model.Country}",
                Items = order.Items.Select(oi => new OrderItemViewModel {
                    ProductId = oi.ProductId,
                    Name = GetProductName(oi.ProductId), // Get product name if possible
                    Price = oi.UnitPrice.Amount,
                    Quantity = oi.Quantity,
                    Total = oi.TotalPrice.Amount
                }).ToList(),
                Total = order.TotalPrice.Amount
            };

            return View("Receipt", receiptModel);
        }        private string GetProductName(Guid productId)
        {
            try
            {
                // Try to get the product from the catalog service
                var product = _catalogService.GetProductByIdAsync(productId).Result;
                
                // Return the product name if found, otherwise return a fallback
                return product != null 
                    ? product.Name 
                    : "Product #" + productId.ToString().Substring(0, 8);
            }
            catch (Exception)
            {
                // If there's an error (like a timeout), fall back to a generic name
                return "Product #" + productId.ToString().Substring(0, 8);
            }
        }[HttpGet]
        public IActionResult Receipt(ReceiptViewModel model)
        {
            return View(model);
        }
    }
}
