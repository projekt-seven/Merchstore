using System.Security.Claims;
using MerchStore.Domain.Interfaces;
using MerchStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Se till att denna finns

namespace MerchStore.Controllers;

public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserRepository userRepository, ILogger<AccountController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
        // Här börjar scope:en som lägger till LoginAttempt och Username
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["LoginAttempt"] = true,
            ["Username"] = model.Username ?? "Anonymous"
        }))
        {
            // Lägg till denna logg direkt här
            _logger.LogInformation("Structured login test – with LoginAttempt scope");

            // Fortsätt sedan som vanligt
            _logger.LogInformation("Login attempt started");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login failed due to invalid model state");
                return View(model);
            }

            var user = await _userRepository.GetByUsernameAsync(model.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                // Inloggning lyckad – logga det
                _logger.LogInformation("Login successful for user {Username} with role {Role}", user.Username, user.Role);

                // Cookie-biten
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Dashboard", "Admin");
            }

            // Misslyckad inloggning
            _logger.LogWarning("Login failed - invalid credentials for user: {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name ?? "Unknown";

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        _logger.LogInformation("User {Username} logged out", username);

        return RedirectToAction("Index", "Home");
    }
}
