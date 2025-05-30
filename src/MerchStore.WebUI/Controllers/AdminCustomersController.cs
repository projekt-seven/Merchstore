using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.DTOs;
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
            var customers = await _customerService.GetCustomerListAsync();
            return View(customers);
        }

        // GET: /AdminCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminCustomers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _customerService.CreateAsync(model);
            TempData["SuccessMessage"] = "Customer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminCustomers/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // POST: /AdminCustomers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerDto model)
        {
            if (id != model.Id || !ModelState.IsValid)
                return View(model);

            await _customerService.UpdateAsync(model);
            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminCustomers/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // GET: /AdminCustomers/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // POST: /AdminCustomers/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _customerService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Customer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
