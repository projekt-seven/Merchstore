using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.DTOs;
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
            var orders = await _orderService.GetOrderListAsync();
            return View(orders);
        }

        // GET: /AdminOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _orderService.CreateAsync(model);
            TempData["SuccessMessage"] = "Order created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminOrders/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: /AdminOrders/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, OrderDto model)
        {
            if (id != model.Id || !ModelState.IsValid)
                return View(model);

            await _orderService.UpdateAsync(model);
            TempData["SuccessMessage"] = "Order updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminOrders/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: /AdminOrders/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: /AdminOrders/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _orderService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
