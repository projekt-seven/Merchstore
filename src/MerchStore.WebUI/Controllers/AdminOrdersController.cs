using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MerchStore.WebUI.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminOrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /AdminOrders
        public async Task<IActionResult> Index()
        {
            // Hämtar lista av orders via tjänstelagret
            var orders = await _orderService.GetOrderListAsync();

            // Skickar listan vidare till vyn
            return View(orders);
        }
    }
}
