using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchStore.Models;

namespace MerchStore.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Users()
    {
        // In a real application, this would fetch from a database
        var users = new List<string> { "admin", "john.doe" };
        return View(users);
    }
}