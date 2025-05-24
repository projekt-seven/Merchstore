using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MerchStore.WebUI.Models;

namespace MerchStore.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var user = User.Identity?.Name ?? "Anonymous";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Action"] = "Index",
            ["User"] = user
        }))
        {
            // Unstructured log (bad for searchability)
            _logger.LogInformation($"Index action called by {user}");

            // Structured log (good!)
            _logger.LogInformation("Index action called by user: {User}", user);
        }

        return View();
    }

    public IActionResult Privacy()
    {
        var user = User.Identity?.Name ?? "Anonymous";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Action"] = "Privacy",
            ["User"] = user
        }))
        {
            _logger.LogInformation("Privacy page accessed by: {User}", user);
        }

        return View();
    }

    public IActionResult About()
    {
        var user = User.Identity?.Name ?? "Anonymous";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Action"] = "About",
            ["User"] = user
        }))
        {
            _logger.LogInformation("About page accessed by: {User}", user);
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Action"] = "Error",
            ["RequestId"] = requestId
        }))
        {
            _logger.LogError("Error page displayed for request: {RequestId}", requestId);
        }

        return View(new ErrorViewModel { RequestId = requestId });
    }

    [Authorize]
    public IActionResult WhoAmI()
    {
        var user = User.Identity?.Name ?? "Unknown";

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Action"] = "WhoAmI",
            ["User"] = user
        }))
        {
            _logger.LogInformation("WhoAmI accessed by authenticated user: {User}", user);
        }

        return View();
    }
}
