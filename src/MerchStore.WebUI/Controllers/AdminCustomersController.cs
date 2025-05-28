using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MerchStore.WebUI.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminCustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public AdminCustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: /AdminCustomers
        public async Task<IActionResult> Index()
        {
            // Hämtar lista av customers via tjänstelagret
            var customers = await _customerService.GetCustomerListAsync();

            // Skickar listan vidare till vyn
            return View(customers);
        }
    }
}
