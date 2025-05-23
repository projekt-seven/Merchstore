using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Products.Commands;
using MerchStore.Application.Products.Queries;
using MediatR;
using MerchStore.Models;

namespace MerchStore.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Dashboard() => View();

    public IActionResult Users()
    {
        var users = new List<string> { "admin", "john.doe" };
        return View(users);
    }


    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }
    // === Produkter ===
    public async Task<IActionResult> Products()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return View(products);
    }

    public IActionResult Orders()
    {
        // TODO: Ersätt med riktig orderhämtning
        return View("Orders", new List<object>());
    }

    public IActionResult Customers()
    {
        // TODO: Ersätt med riktig kundhämtning
        return View("Customers", new List<object>());
    }

    // visa en produkt
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product == null)
            return NotFound();

        return View(product);
    }

    // skapa en ny produkt
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, [FromForm] string Tags)
    {
        if (!ModelState.IsValid)
            return View(request);

        if (!string.IsNullOrWhiteSpace(Tags))
            request.Tags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(t => t.Trim())
                               .ToList();

        try
        {
            await _mediator.Send(new CreateProductCommand(request));
            return RedirectToAction(nameof(Index)); // visa listan efteråt
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }
    // redigera en produkt
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product == null) return NotFound();

        // Förifyll formuläret med produkten
        var request = new CreateProductRequest
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            Tags = product.Tags
        };

        ViewBag.ProductId = id;
        return View(request);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, CreateProductRequest request, [FromForm] string Tags)
    {
        if (!ModelState.IsValid) return View(request);

        if (!string.IsNullOrWhiteSpace(Tags))
            request.Tags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();

        await _mediator.Send(new UpdateProductCommand(id, request));
        return RedirectToAction(nameof(Products));
    }
    // ta bort en produkt
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null)
            return NotFound();

        return View(product);
    }

    // ta bort en produkt - bekräfta
    // confirmed delete

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var success = await _mediator.Send(new DeleteProductCommand(id));
        if (!success)
            return NotFound();

        TempData["SuccessMessage"] = "Produkten har raderats.";
        return RedirectToAction(nameof(Products));
    }
}